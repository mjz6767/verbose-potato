-- Read-only, per-cell neutral-component audit for the installed Aseprite.
local sprite = app.activeSprite
assert(sprite and sprite.colorMode == ColorMode.RGB, "An RGBA atlas is required")
local image = Image(sprite)
local columns = assert(tonumber(app.params.columns), "columns is required")
local rows = assert(tonumber(app.params.rows), "rows is required")
local size = image.width // columns
assert(size * rows == image.height, "Square cells are required")
local minimum = tonumber(app.params.minimum or "180")
local tolerance = tonumber(app.params.tolerance or "20")
local pc = app.pixelColor
local report = {}
for cell = 0, columns * rows - 1 do
  local ox, oy = (cell % columns) * size, (cell // columns) * size
  local seen, components = {}, {}
  local function candidate(x, y)
    local p = image:getPixel(ox + x, oy + y)
    local r, g, b = pc.rgbaR(p), pc.rgbaG(p), pc.rgbaB(p)
    return pc.rgbaA(p) > 0 and math.min(r,g,b) >= minimum and math.max(r,g,b)-math.min(r,g,b) <= tolerance
  end
  for y = 0, size - 1 do
    for x = 0, size - 1 do
      local id = y * size + x
      if not seen[id] and candidate(x,y) then
        local queue, head = {id}, 1
        seen[id] = true
        local left, right, top, bottom = x, x, y, y
        local count, edge, bright = 0, false, 0
        while head <= #queue do
          local n = queue[head]; head = head + 1
          local xx, yy = n % size, n // size
          count = count + 1
          left, right, top, bottom = math.min(left,xx), math.max(right,xx), math.min(top,yy), math.max(bottom,yy)
          local p = image:getPixel(ox+xx,oy+yy)
          if math.min(pc.rgbaR(p),pc.rgbaG(p),pc.rgbaB(p)) >= 225 then bright=bright+1 end
          for _, delta in ipairs({{-1,0},{1,0},{0,-1},{0,1}}) do
            local nx,ny=xx+delta[1],yy+delta[2]
            if nx>=0 and ny>=0 and nx<size and ny<size then
              local neighbor=ny*size+nx
              if pc.rgbaA(image:getPixel(ox+nx,oy+ny)) == 0 then edge=true end
              if not seen[neighbor] and candidate(nx,ny) then
                seen[neighbor]=true; queue[#queue+1]=neighbor
              end
            end
          end
        end
        if count >= 20 then
          components[#components+1]={ seed={x,y}, bounds={left,top,right+1,bottom+1}, count=count, bright=bright, touchesTransparency=edge }
        end
      end
    end
  end
  report[#report+1]={cell=cell,components=components}
end
print(json.encode(report))
