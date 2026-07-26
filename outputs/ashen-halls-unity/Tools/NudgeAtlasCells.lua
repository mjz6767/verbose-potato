local sprite = app.activeSprite
if sprite == nil then
  error("NudgeAtlasCells requires an open sprite")
end

local columns = tonumber(app.params["columns"] or "5")
local rows = tonumber(app.params["rows"] or "4")
local dx = tonumber(app.params["dx"] or "0")
local dy = tonumber(app.params["dy"] or "0")
local cellsText = app.params["cells"] or ""

if sprite.width % columns ~= 0 or sprite.height % rows ~= 0 then
  error("Sprite dimensions must divide evenly by the requested grid")
end

local selected = {}
for token in string.gmatch(cellsText, "[^,]+") do
  local index = tonumber(token)
  if index == nil or index < 0 or index >= columns * rows then
    error("Invalid atlas cell index: " .. token)
  end
  selected[index] = true
end

local cel = sprite.cels[1]
if cel == nil then
  error("Sprite has no image cel")
end

local source = cel.image:clone()
local output = source:clone()
local cellWidth = sprite.width // columns
local cellHeight = sprite.height // rows
local transparent = app.pixelColor.rgba(0, 0, 0, 0)

for index, _ in pairs(selected) do
  local column = index % columns
  local row = index // columns
  local left = column * cellWidth
  local top = row * cellHeight

  for y = 0, cellHeight - 1 do
    for x = 0, cellWidth - 1 do
      output:drawPixel(left + x, top + y, transparent)
    end
  end

  for y = 0, cellHeight - 1 do
    for x = 0, cellWidth - 1 do
      local targetX = x + dx
      local targetY = y + dy
      if targetX >= 0 and targetX < cellWidth and targetY >= 0 and targetY < cellHeight then
        output:drawPixel(left + targetX, top + targetY, source:getPixel(left + x, top + y))
      end
    end
  end
end

cel.image = output
