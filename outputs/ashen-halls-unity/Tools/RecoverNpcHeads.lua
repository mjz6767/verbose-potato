-- Native Aseprite recovery from the intact authoring sheets. The old equal-row
-- crop cut through heads before padding them, so runtime gutters hid the defect.
local sprite = app.activeSprite
assert(sprite and sprite.colorMode == ColorMode.RGB, 'Open the approved RGBA atlas')
local kind = app.params.kind
assert(kind == 'named' or kind == 'citizen', 'Unknown atlas kind')
local source = Image{fromFile=app.params.source}
local original = Image(sprite)
local result = original:clone()
local pc = app.pixelColor
local size, columns, gutter = 256, 5, 18
local recipes = {
  {14,980,615,1254,914,{{84,183},{132,201},{175,201}}},
  {16,251,910,502,1254,{{107,58},{130,197}}},
  {17,502,910,752,1254,{}},
  {18,752,910,1003,1254,{{134,210}}},
  {19,1003,910,1254,1254,{{136,200}}},
}
if kind == 'citizen' then
  size,columns,gutter = 384,4,20
  recipes = {
    {4,0,480,384,1024,{{217,323}}},
    {5,384,480,768,1024,{{106,70},{136,178},{214,328}}},
    {6,768,480,1152,1024,{{142,230},{215,321}}},
    {7,1152,480,1536,1024,{{105,80},{123,210},{190,315}}},
  }
end
assert(sprite.width == columns*size and sprite.height == (kind=='named' and 4 or 2)*size,
  'Runtime geometry changed')
local function neutral(p,minimum,tolerance)
  local r,g,b=pc.rgbaR(p),pc.rgbaG(p),pc.rgbaB(p)
  return math.min(r,g,b)>=minimum and math.max(r,g,b)-math.min(r,g,b)<=tolerance
end
local function clearMatte(image,seeds)
  local w,h=image.width,image.height
  local mask,queue,head={},{},1
  local function enqueue(x,y)
    if x<0 or y<0 or x>=w or y>=h then return end
    local id=y*w+x
    if mask[id] then return end
    local p=image:getPixel(x,y)
    if pc.rgbaA(p)>0 and not neutral(p,180,20) then return end
    mask[id]=true;queue[#queue+1]=id
  end
  for x=0,w-1 do enqueue(x,0);enqueue(x,h-1) end
  for y=0,h-1 do enqueue(0,y);enqueue(w-1,y) end
  for _,v in ipairs(seeds or {}) do
    assert(pc.rgbaA(image:getPixel(v[1],v[2]))==0 or neutral(image:getPixel(v[1],v[2]),180,20),
      'Negative-space seed needs review: '..v[1]..','..v[2])
    enqueue(v[1],v[2])
  end
  while head<=#queue do
    local id=queue[head];head=head+1
    local x,y=id%w,id//w
    enqueue(x-1,y);enqueue(x+1,y);enqueue(x,y-1);enqueue(x,y+1)
  end
  local fringe={}
  for y=1,h-2 do for x=1,w-2 do
    local id=y*w+x
    if not mask[id] and neutral(image:getPixel(x,y),140,22)
      and (mask[id-1] or mask[id+1] or mask[id-w] or mask[id+w]) then fringe[id]=true end
  end end
  for id,_ in pairs(mask) do image:putPixel(id%w,id//w,0) end
  for id,_ in pairs(fringe) do image:putPixel(id%w,id//w,0) end
end
local function softenGround(image)
  local w,h=image.width,image.height
  local start=math.floor(h*0.845)
  local mask,queue,head={},{},1
  local function enqueue(x,y)
    if x<0 or x>=w or y<start or y>=h then return end
    local id=y*w+x
    if mask[id] then return end
    local p=image:getPixel(x,y)
    if pc.rgbaA(p)>0 and not neutral(p,35,18) then return end
    mask[id]=true;queue[#queue+1]=id
  end
  for x=0,w-1 do enqueue(x,h-1) end
  for y=start,h-1 do enqueue(0,y);enqueue(w-1,y) end
  while head<=#queue do
    local id=queue[head];head=head+1
    local x,y=id%w,id//w
    enqueue(x-1,y);enqueue(x+1,y);enqueue(x,y-1);enqueue(x,y+1)
  end
  for id,_ in pairs(mask) do
    local x,y=id%w,id//w
    local p=image:getPixel(x,y)
    if pc.rgbaA(p)>0 then
      local mean=(pc.rgbaR(p)+pc.rgbaG(p)+pc.rgbaB(p))/3
      image:putPixel(x,y,pc.rgba(0,0,0,math.min(70,math.floor(pc.rgbaA(p)*(1-mean/255)*0.65+0.5))))
    end
  end
end
local function retainSubject(image,index)
  local w,h=image.width,image.height
  local visited,components={},{}
  for y=0,h-1 do for x=0,w-1 do
    local start=y*w+x
    if not visited[start] and pc.rgbaA(image:getPixel(x,y))>=24 then
      local queue,head={start},1
      local l,t,r,b=x,y,x,y
      visited[start]=true
      local function enqueue(nx,ny)
        if nx<0 or ny<0 or nx>=w or ny>=h then return end
        local id=ny*w+nx
        if visited[id] or pc.rgbaA(image:getPixel(nx,ny))<24 then return end
        visited[id]=true;queue[#queue+1]=id
      end
      while head<=#queue do
        local id=queue[head];head=head+1
        local px,py=id%w,id//w
        l,t,r,b=math.min(l,px),math.min(t,py),math.max(r,px),math.max(b,py)
        enqueue(px-1,py);enqueue(px+1,py);enqueue(px,py-1);enqueue(px,py+1)
      end
      components[#components+1]={points=queue,l=l,t=t,r=r,b=b}
    end
  end end
  table.sort(components,function(a,b) return #a.points>#b.points end)
  assert(#components>0,'Empty source crop')
  local main=components[1]
  assert(main.l>1 and main.t>1 and main.r<w-2 and main.b<h-2,
    'Source crop cuts the main silhouette: '..index..' bounds='..main.l..','..main.t..','..main.r..','..main.b..' canvas='..w..','..h)
  local kept=Image(w,h,ColorMode.RGB)
  for i,c in ipairs(components) do
    local boundary=c.l==0 or c.t==0 or c.r==w-1 or c.b==h-1
    if i==1 or (not boundary and ((c.l<=main.r and c.r>=main.l) or #c.points>=#main.points*0.1)) then
      for _,id in ipairs(c.points) do kept:putPixel(id%w,id//w,image:getPixel(id%w,id//w)) end
    end
  end
  return kept
end
local records={}
for _,recipe in ipairs(recipes) do
  local index,left,top,right,bottom=table.unpack(recipe,1,5)
  local crop=Image(source,Rectangle(left,top,right-left,bottom-top))
  clearMatte(crop)
  if app.params.debug then crop:saveAs(app.params.output..'.cell-'..index..'.png') end
  crop=retainSubject(crop,index)
  local bounds=crop:shrinkBounds()
  assert(bounds.x>1 and bounds.y>1 and bounds.x+bounds.width<crop.width-1
    and bounds.y+bounds.height<crop.height-1,'Source crop cuts a subject: '..index)
  -- Trim only after proving the full silhouette is inside its source crop.
  local figure=Image(crop,bounds)
  local height=size-gutter*2
  local width=math.floor(figure.width*height/figure.height+0.5)
  assert(width<=size-gutter*2,'Restored figure is too wide')
  figure:resize{width=width,height=height,method='bilinear'}
  local cell=Image(size,size,ColorMode.RGB)
  cell:drawImage(figure,Point((size-width)//2,gutter))
  if app.params.debug then cell:saveAs(app.params.output..'.cell-'..index..'-normalized.png') end
  -- Explicit, reviewed negative-space locations in the full-height runtime cell.
  -- They target enclosed paper/matte pockets, never similarly bright clothing.
  local seeds=recipe[6]
  clearMatte(cell,seeds)
  softenGround(cell)
  local ox,oy=(index%columns)*size,(index//columns)*size
  for y=0,size-1 do for x=0,size-1 do result:putPixel(ox+x,oy+y,cell:getPixel(x,y)) end end
  records[#records+1]={cell=index,sourceRect={left,top,right,bottom},
    fullSourceBounds={left+bounds.x,top+bounds.y,bounds.width,bounds.height},
    normalizedSize={width,height},negativeSpaceSeeds=seeds}
end
local changed={}
for _,r in ipairs(recipes) do changed[r[1]]=true end
local untouchedPixels=0
for y=0,result.height-1 do for x=0,result.width-1 do
  local cell=(y//size)*columns+x//size
  if not changed[cell] then
    assert(result:getPixel(x,y)==original:getPixel(x,y),'Untargeted artwork changed')
    untouchedPixels=untouchedPixels+1
  end
end end
app.transaction('Recover intact NPC heads',function()
  sprite.layers[1].name='Approved v2.26.1 reference (hidden)'
  sprite.layers[1].isVisible=false
  local layer=sprite:newLayer()
  layer.name='Full silhouettes - restored original pixels'
  sprite:newCel(layer,1,result,Point(0,0))
end)
assert(sprite:saveAs(app.params.editable),'Native save failed')
assert(result:saveAs(app.params.output),'Runtime export failed')
print(json.encode({kind=kind,cells=records,width=sprite.width,height=sprite.height,
  untouchedPixelsVerified=untouchedPixels}))
