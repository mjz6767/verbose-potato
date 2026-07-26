local sprite = app.activeSprite
if sprite == nil then
  error("StrengthenAtlasCellAlpha requires an active sprite")
end

if sprite.colorMode ~= ColorMode.RGB then
  error("StrengthenAtlasCellAlpha requires an RGBA sprite")
end

local columns = tonumber(app.params["columns"] or "5")
local rows = tonumber(app.params["rows"] or "4")
local coreAlpha = tonumber(app.params["coreAlpha"] or "96")
local edgeAlpha = tonumber(app.params["edgeAlpha"] or "48")
local selected = {}

for token in string.gmatch(app.params["cells"] or "", "[^,]+") do
  local index = tonumber(token)
  if index ~= nil then selected[index] = true end
end

if columns == nil or rows == nil or columns < 1 or rows < 1 then
  error("columns and rows must be positive integers")
end

local changed = 0
app.transaction("Strengthen structural alpha", function()
  for _, cel in ipairs(sprite.cels) do
    local image = cel.image
    local origin = cel.position
    for y = 0, image.height - 1 do
      local canvasY = origin.y + y
      if canvasY >= 0 and canvasY < sprite.height then
        local row = math.min(rows - 1, math.floor(canvasY * rows / sprite.height))
        for x = 0, image.width - 1 do
          local canvasX = origin.x + x
          if canvasX >= 0 and canvasX < sprite.width then
            local column = math.min(columns - 1, math.floor(canvasX * columns / sprite.width))
            local index = row * columns + column
            if selected[index] then
              local pixel = image:getPixel(x, y)
              local alpha = app.pixelColor.rgbaA(pixel)
              local nextAlpha = alpha
              if alpha >= coreAlpha then
                nextAlpha = 255
              elseif alpha >= edgeAlpha then
                nextAlpha = math.min(223, math.floor(alpha * 1.55 + 24 + 0.5))
              end

              if nextAlpha ~= alpha then
                image:putPixel(x, y, app.pixelColor.rgba(
                  app.pixelColor.rgbaR(pixel),
                  app.pixelColor.rgbaG(pixel),
                  app.pixelColor.rgbaB(pixel),
                  nextAlpha))
                changed = changed + 1
              end
            end
          end
        end
      end
    end
  end
end)

print(string.format(
  "StrengthenAtlasCellAlpha: cells=%s grid=%dx%d changed=%d",
  app.params["cells"] or "",
  columns,
  rows,
  changed))

local output = app.params["output"]
if output ~= nil and output ~= "" then
  local saved = sprite:saveAs(output)
  if not saved then error("Could not save repaired atlas: " .. output) end
end
