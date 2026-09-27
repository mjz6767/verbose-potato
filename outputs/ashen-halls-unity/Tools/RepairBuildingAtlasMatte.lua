-- Reviewed Aseprite repair for the nine v2.21 Midgaard architecture cells.
-- Not a global white color-key: bounded exterior fringe and authored sign gaps.
local sprite=app.activeSprite
assert(sprite and sprite.colorMode==ColorMode.RGB, "RGBA town atlas required")
assert(sprite.width==1280 and sprite.height==1024, "Expected 5x4 256px cells")
assert(#sprite.frames==1 and #sprite.cels==1, "Expected flat source export")
local source=Image(sprite)
local repaired=source:clone()
local pc=app.pixelColor
local size=256
local cells={0,1,3,4,5,6,7,11,14}
-- Smoke is intentionally light and touches transparency; preserve its full
-- source pixels, including soft/bright edges. Coordinates are cell-local.
local protected={
  [4]={{61,22,99,59}},
  [6]={{147,24,185,61}},
  [14]={{155,17,194,57}},
}
-- Reviewed negative space enclosed by hanging-sign brackets. These white
-- islands cannot be reached from the cell exterior, exactly as in the NPCs.
local holes={
  [0]={{220,144}}, [3]={{210,136},{200,137},{195,148}},
  [4]={{33,131}}, [5]={{38,128}}, [6]={{37,125}},
  [7]={{43,116},{64,118}}, [14]={{23,135}},
}
local report={}
for _,cell in ipairs(cells) do
  local ox,oy=(cell%5)*size,(cell//5)*size
  local function pixel(x,y) return source:getPixel(ox+x,oy+y) end
  local function protect(x,y)
    for _,r in ipairs(protected[cell] or {}) do
      if x>=r[1] and y>=r[2] and x<r[3] and y<r[4] then return true end
    end
    return false
  end
  local function neutral(p,minimum,tolerance)
    local r,g,b=pc.rgbaR(p),pc.rgbaG(p),pc.rgbaB(p)
    return math.min(r,g,b)>=minimum and math.max(r,g,b)-math.min(r,g,b)<=tolerance
  end
  -- Distance from original transparency limits the repair to the thin matte.
  -- It never follows a light stone seam deep into the building facade.
  local distance,queue,head={},{},1
  for y=0,size-1 do for x=0,size-1 do
    local id=y*size+x
    if pc.rgbaA(pixel(x,y))==0 then distance[id]=0;queue[#queue+1]=id end
  end end
  while head<=#queue do
    local id=queue[head];head=head+1
    if distance[id]<2 then
      local x,y=id%size,id//size
      for _,d in ipairs({{-1,0},{1,0},{0,-1},{0,1}}) do
        local nx,ny=x+d[1],y+d[2]
        local next=ny*size+nx
        if nx>=0 and ny>=0 and nx<size and ny<size and distance[next]==nil then
          distance[next]=distance[id]+1;queue[#queue+1]=next
        end
      end
    end
  end
  local mask,pending,first={},{},1
  local function enqueue(x,y)
    if x<0 or y<0 or x>=size or y>=size or protect(x,y) then return end
    local id=y*size+x
    if mask[id] then return end
    local p=pixel(x,y)
    if pc.rgbaA(p)>0 and (not distance[id] or not neutral(p,180,24)) then return end
    mask[id]=true;pending[#pending+1]=id
  end
  for x=0,size-1 do enqueue(x,0);enqueue(x,size-1) end
  for y=0,size-1 do enqueue(0,y);enqueue(size-1,y) end
  while first<=#pending do
    local id=pending[first];first=first+1
    local x,y=id%size,id//size
    enqueue(x-1,y);enqueue(x+1,y);enqueue(x,y-1);enqueue(x,y+1)
  end
  local holeMask,holeQueue,holeHead={},{},1
  local function enqueueHole(x,y)
    if x<0 or y<0 or x>=size or y>=size or protect(x,y) then return end
    local id=y*size+x
    if holeMask[id] then return end
    local p=pixel(x,y)
    if pc.rgbaA(p)==0 or not neutral(p,180,24) then return end
    holeMask[id]=true;mask[id]=true;holeQueue[#holeQueue+1]=id
  end
  for _,point in ipairs(holes[cell] or {}) do
    assert(pc.rgbaA(pixel(point[1],point[2]))>0 and neutral(pixel(point[1],point[2]),180,24),"Reviewed sign gap changed")
    enqueueHole(point[1],point[2])
  end
  while holeHead<=#holeQueue do
    local id=holeQueue[holeHead];holeHead=holeHead+1
    local x,y=id%size,id//size
    enqueueHole(x-1,y);enqueueHole(x+1,y);enqueueHole(x,y-1);enqueueHole(x,y+1)
  end
  -- A single non-progressive darker antialias ring, still no deeper than
  -- two pixels from original transparency except beside reviewed sign gaps.
  -- Interior highlights stay exact.
  local fringe={}
  for y=1,size-2 do for x=1,size-2 do
    local id=y*size+x
    local bordersHole=holeMask[id-1] or holeMask[id+1] or holeMask[id-size] or holeMask[id+size]
    if not mask[id] and (distance[id] or bordersHole) and not protect(x,y) then
      local p=pixel(x,y)
      if pc.rgbaA(p)>0 and neutral(p,140,26)
        and (mask[id-1] or mask[id+1] or mask[id-size] or mask[id+size]) then fringe[id]=true end
    end
  end end
  local removed=0
  for y=0,size-1 do for x=0,size-1 do
    local id=y*size+x
    if pc.rgbaA(pixel(x,y))>0 and (mask[id] or fringe[id]) then
      repaired:putPixel(ox+x,oy+y,0);removed=removed+1
    end
  end end
  assert(removed>100 and removed<2500, "Unexpected contour repair count in cell "..cell..": "..removed)
  report[#report+1]={cell=cell,removed=removed,protectedRegions=protected[cell] or {},holeSeeds=holes[cell] or {}}
end
app.transaction("Clean reviewed building contours",function()
  local original=sprite.layers[1]
  original.name="Original v2.21 - reference (hidden)"
  original.isVisible=false
  local layer=sprite:newLayer()
  layer.name="Buildings - clean transparent contours"
  sprite:newCel(layer,1,repaired,Point(0,0))
end)
assert(app.params.editable and app.params.output,"Editable and PNG paths required")
assert(sprite:saveAs(app.params.editable),"Native save failed")
assert(repaired:saveAs(app.params.output),"PNG save failed")
print(json.encode({width=sprite.width,height=sprite.height,cells=report}))
