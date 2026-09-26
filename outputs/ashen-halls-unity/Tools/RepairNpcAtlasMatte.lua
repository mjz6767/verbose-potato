-- Aseprite-only repair of the reviewed v2.21 NPC exports. This is not a
-- general white color-key: exterior connectivity and authored negative-space
-- seeds distinguish matte from pale clothes, lanterns, hair and steel.
local sprite = app.activeSprite
assert(sprite and sprite.colorMode == ColorMode.RGB, "An RGBA atlas is required")
assert(#sprite.frames == 1 and #sprite.cels == 1, "Expected the flat v2.21 runtime export")
local kind = app.params.kind
assert(kind == "citizen" or kind == "named", "kind must be citizen or named")
local columns, rows, size = 5, 4, 256
if kind == "citizen" then columns, rows, size = 4, 2, 384 end
assert(sprite.width == columns*size and sprite.height == rows*size, "Unexpected atlas geometry")
local original = Image(sprite)
local repaired = original:clone()
local pc = app.pixelColor
-- Coordinates are cell-local, top-down, reviewed against the retained source.
-- Small isolated bright regions (eyes, fish scales, lamp cores) are not seeds.
local holes = kind == "citizen" and {
  [0]={{162,186},{211,306},{103,150}},
  [1]={{158,233},{211,292}},
  [2]={},
  [3]={{214,278},{263,160}},
  [4]={{218,310}},
  [5]={{102,49},{134,170},{214,323}},
  [6]={{155,180},{219,291}},
  [7]={{102,63},{134,164},{191,296}},
} or {
  [0]={{82,135},{84,201},{123,203}},
  [1]={{97,142},{138,206},{103,207},{173,211}},
  [2]={}, [3]={}, [4]={{98,149}}, [5]={{148,196}},
  [6]={{150,193}}, [7]={{119,173},{152,129}}, [8]={{116,205}}, [9]={},
  [10]={}, [11]={{128,183}}, [12]={{135,191}},
  [13]={{160,175},{123,176}},
  [14]={{81,166},{132,177},{172,184},{93,147},{171,159}},
  [15]={{165,154},{133,175}}, [16]={{108,39},{130,188}},
  [17]={}, [18]={{132,184}}, [19]={{136,187}},
}
local counts = {}
for cell=0,columns*rows-1 do
  local ox,oy=(cell%columns)*size,(cell//columns)*size
  local mask, queue, head = {}, {}, 1
  local function channels(x,y)
    local p=original:getPixel(ox+x,oy+y)
    return pc.rgbaR(p),pc.rgbaG(p),pc.rgbaB(p),pc.rgbaA(p)
  end
  local function neutral(r,g,b,minimum,tolerance)
    return math.min(r,g,b)>=minimum and math.max(r,g,b)-math.min(r,g,b)<=tolerance
  end
  local function enqueue(x,y)
    if x<0 or y<0 or x>=size or y>=size then return end
    local id=y*size+x
    if mask[id] then return end
    local r,g,b,a=channels(x,y)
    if a~=0 and not neutral(r,g,b,180,20) then return end
    mask[id]=true; queue[#queue+1]=id
  end
  for x=0,size-1 do enqueue(x,0); enqueue(x,size-1) end
  for y=0,size-1 do enqueue(0,y); enqueue(size-1,y) end
  for _,point in ipairs(holes[cell]) do
    local r,g,b,a=channels(point[1],point[2])
    assert(a>0 and neutral(r,g,b,180,20), "Reviewed hole seed changed: cell "..cell)
    enqueue(point[1],point[2])
  end
  while head<=#queue do
    local id=queue[head];head=head+1
    local x,y=id%size,id//size
    enqueue(x-1,y);enqueue(x+1,y);enqueue(x,y-1);enqueue(x,y+1)
  end
  -- Clear one lower-value neutral antialias ring bordering the removed matte.
  -- Read only the original mask, so this cannot progressively eat a sprite.
  local fringe={}
  for y=1,size-2 do for x=1,size-2 do
    local id=y*size+x
    local r,g,b,a=channels(x,y)
    if not mask[id] and a>0 and neutral(r,g,b,140,22)
      and (mask[id-1] or mask[id+1] or mask[id-size] or mask[id+size]) then
      fringe[id]=true
    end
  end end
  local removed, shadows, unchanged=0,0,0
  local shadowTop=kind=="citizen" and 325 or 217
  local shadowMask, shadowQueue, shadowHead = {}, {}, 1
  local function enqueueShadow(x,y)
    if x<0 or x>=size or y<shadowTop or y>=size then return end
    local id=y*size+x
    if shadowMask[id] then return end
    local r,g,b,a=channels(x,y)
    if a>0 and not mask[id] and not fringe[id] and not neutral(r,g,b,35,18) then return end
    shadowMask[id]=true;shadowQueue[#shadowQueue+1]=id
  end
  for x=0,size-1 do enqueueShadow(x,size-1) end
  for y=shadowTop,size-1 do enqueueShadow(0,y);enqueueShadow(size-1,y) end
  while shadowHead<=#shadowQueue do
    local id=shadowQueue[shadowHead];shadowHead=shadowHead+1
    local x,y=id%size,id//size
    enqueueShadow(x-1,y);enqueueShadow(x+1,y);enqueueShadow(x,y-1);enqueueShadow(x,y+1)
  end
  for y=0,size-1 do for x=0,size-1 do
    local id=y*size+x
    local r,g,b,a=channels(x,y)
    local next=original:getPixel(ox+x,oy+y)
    if a>0 and (mask[id] or fringe[id]) then
      next=0;removed=removed+1
    elseif a>0 and shadowMask[id] then
      -- The old contact shadow was composited over white. Retain a subdued
      -- black shadow with real alpha, never an opaque gray/white footplate.
      local opacity=math.min(70,math.floor(a*(1-(r+g+b)/(3*255))*0.65+0.5))
      next=pc.rgba(0,0,0,opacity);shadows=shadows+1
    else unchanged=unchanged+1 end
    repaired:putPixel(ox+x,oy+y,next)
  end end
  assert(removed>30, "Expected to remove the known matte in every cell")
  counts[#counts+1]={cell=cell,removed=removed,shadowPixels=shadows,unchanged=unchanged}
end
-- Both the unmodified reference layer and corrected working layer are retained
-- in the native file. Only the working layer is visible in the PNG export.
app.transaction("Repair reviewed NPC matte", function()
  local reference=sprite.layers[1]
  reference.name="Original v2.21 - reference (hidden)"
  reference.isVisible=false
  local layer=sprite:newLayer()
  layer.name="Corrected transparent NPC sprites"
  sprite:newCel(layer,1,repaired,Point(0,0))
end)
assert(app.params.editable and app.params.output, "Separate editable and PNG outputs are required")
assert(sprite:saveAs(app.params.editable), "Could not save editable sprite")
assert(repaired:saveAs(app.params.output), "Could not save runtime PNG")
print(json.encode({kind=kind,cells=counts,width=sprite.width,height=sprite.height}))
