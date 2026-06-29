(function () {
  "use strict";

  const canvas = document.getElementById("gameCanvas");
  const ctx = canvas.getContext("2d");
  const depthValue = document.getElementById("depthValue");
  const goldValue = document.getElementById("goldValue");
  const suppliesValue = document.getElementById("suppliesValue");
  const elixirValue = document.getElementById("elixirValue");
  const modeValue = document.getElementById("modeValue");
  const turnBadge = document.getElementById("turnBadge");
  const roundBadge = document.getElementById("roundBadge");
  const actionBadge = document.getElementById("actionBadge");
  const partyList = document.getElementById("partyList");
  const enemyList = document.getElementById("enemyList");
  const logList = document.getElementById("logList");
  const sceneBanner = document.getElementById("sceneBanner");

  const buttons = {
    save: document.getElementById("saveBtn"),
    load: document.getElementById("loadBtn"),
    newGame: document.getElementById("newBtn"),
    camp: document.getElementById("campBtn"),
    descend: document.getElementById("descendBtn"),
    potion: document.getElementById("potionBtn"),
    move: document.getElementById("moveBtn"),
    attack: document.getElementById("attackBtn"),
    cast: document.getElementById("castBtn"),
    guard: document.getElementById("guardBtn"),
    wait: document.getElementById("waitBtn")
  };

  const SAVE_KEY = "ashen-halls-save-v1";
  const EXPLORE_W = 30;
  const EXPLORE_H = 20;
  const TILE = 32;
  const COMBAT_W = 12;
  const COMBAT_H = 8;
  const COMBAT_CELL = 58;
  const COMBAT_LEFT = 132;
  const COMBAT_TOP = 70;
  const MAX_LOG = 42;

  const dirMap = {
    north: { x: 0, y: -1 },
    south: { x: 0, y: 1 },
    west: { x: -1, y: 0 },
    east: { x: 1, y: 0 }
  };

  let rngSeed = Date.now() % 2147483647;
  let state = null;
  let bannerTimer = null;
  let aiTimer = null;

  function random() {
    rngSeed = (rngSeed * 48271) % 2147483647;
    return rngSeed / 2147483647;
  }

  function randInt(min, max) {
    return Math.floor(random() * (max - min + 1)) + min;
  }

  function clamp(value, min, max) {
    return Math.max(min, Math.min(max, value));
  }

  function keyOf(x, y) {
    return `${x},${y}`;
  }

  function pct(value, max) {
    if (max <= 0) return 0;
    return clamp((value / max) * 100, 0, 100);
  }

  function distance(a, b) {
    return Math.abs(a.x - b.x) + Math.abs(a.y - b.y);
  }

  function alive(list) {
    return list.filter((unit) => unit.hp > 0);
  }

  function makeHero(id, name, role, attrs, spell, range, skills) {
    const hp = attrs.health + 16 + Math.floor(attrs.str / 2);
    const mana = spell ? attrs.int + 8 : Math.max(3, Math.floor(attrs.int / 2));
    return {
      id,
      name,
      role,
      attrs,
      hp,
      maxHp: hp,
      mana,
      maxMana: mana,
      pow: Math.floor(attrs.str / 3) + 4,
      def: Math.floor((attrs.health + attrs.str) / 9),
      agi: Math.floor(attrs.dex / 3) + 2,
      range,
      spell,
      skills: {
        arms: skills.arms || 1,
        missile: skills.missile || 1,
        mend: skills.mend || 1,
        ember: skills.ember || 1,
        hex: skills.hex || 1,
        guard: skills.guard || 1
      }
    };
  }

  function makeParty() {
    return [
      makeHero("maer", "Maer", "shield", { str: 17, int: 7, dex: 9, health: 17 }, null, 1, { arms: 8, guard: 7 }),
      makeHero("cairn", "Cairn", "pike", { str: 15, int: 8, dex: 14, health: 13 }, null, 1, { arms: 7, guard: 4 }),
      makeHero("selka", "Selka", "bow", { str: 10, int: 10, dex: 19, health: 11 }, null, 4, { missile: 8 }),
      makeHero("jory", "Jory", "knife", { str: 11, int: 12, dex: 18, health: 9 }, null, 1, { arms: 5, missile: 4 }),
      makeHero("vesh", "Vesh", "mender", { str: 7, int: 19, dex: 10, health: 14 }, "mend", 1, { mend: 8 }),
      makeHero("oryn", "Oryn", "ember", { str: 8, int: 20, dex: 12, health: 10 }, "ember", 3, { ember: 8 }),
      makeHero("tala", "Tala", "hex", { str: 8, int: 18, dex: 13, health: 11 }, "hex", 3, { hex: 8 }),
      makeHero("rusk", "Rusk", "ward", { str: 14, int: 11, dex: 10, health: 15 }, null, 1, { arms: 5, guard: 8 })
    ];
  }

  function newGame() {
    rngSeed = Date.now() % 2147483647;
    const map = generateMap(1);
    state = {
      mode: "explore",
      depth: 1,
      gold: 28,
      supplies: 5,
      elixirs: 3,
      player: { x: map.start.x, y: map.start.y },
      map,
      party: makeParty(),
      inventory: [],
      combat: null,
      log: []
    };
    pushLog("Eight names are sworn on the old road.", "good");
    render();
  }

  function generateMap(depth) {
    const tiles = [];
    for (let y = 0; y < EXPLORE_H; y += 1) {
      const row = [];
      for (let x = 0; x < EXPLORE_W; x += 1) {
        row.push("wall");
      }
      tiles.push(row);
    }

    let x = Math.floor(EXPLORE_W / 2);
    let y = Math.floor(EXPLORE_H / 2);
    const start = { x, y };
    tiles[y][x] = "floor";

    const steps = 520 + depth * 34;
    for (let i = 0; i < steps; i += 1) {
      const dirs = Object.values(dirMap);
      const d = dirs[randInt(0, dirs.length - 1)];
      x = clamp(x + d.x, 2, EXPLORE_W - 3);
      y = clamp(y + d.y, 2, EXPLORE_H - 3);
      tiles[y][x] = "floor";

      if (random() < 0.17) {
        for (let yy = -1; yy <= 1; yy += 1) {
          for (let xx = -1; xx <= 1; xx += 1) {
            if (random() < 0.68) {
              tiles[clamp(y + yy, 1, EXPLORE_H - 2)][clamp(x + xx, 1, EXPLORE_W - 2)] = "floor";
            }
          }
        }
      }
    }

    const objects = {};
    const open = [];
    for (let yy = 1; yy < EXPLORE_H - 1; yy += 1) {
      for (let xx = 1; xx < EXPLORE_W - 1; xx += 1) {
        if (tiles[yy][xx] === "floor" && distance({ x: xx, y: yy }, start) > 3) {
          open.push({ x: xx, y: yy });
        }
      }
    }

    function place(type, count) {
      for (let i = 0; i < count && open.length; i += 1) {
        const index = randInt(0, open.length - 1);
        const spot = open.splice(index, 1)[0];
        objects[keyOf(spot.x, spot.y)] = { type };
      }
    }

    place("cache", 5 + Math.min(depth, 3));
    place("shrine", 2);
    place("encounter", 5 + depth);
    place("stairs", 1);
    objects[keyOf(start.x, start.y)] = { type: "camp" };
    const town = { x: start.x, y: Math.max(1, start.y - 4) };
    for (let yy = town.y; yy <= start.y; yy += 1) {
      tiles[yy][start.x] = "floor";
    }
    objects[keyOf(town.x, town.y)] = { type: "town" };

    return { tiles, objects, start };
  }

  function pushLog(text, tone) {
    state.log.unshift({ text, tone: tone || "" });
    state.log = state.log.slice(0, MAX_LOG);
  }

  function showBanner(text) {
    sceneBanner.textContent = text;
    sceneBanner.hidden = false;
    clearTimeout(bannerTimer);
    bannerTimer = setTimeout(() => {
      sceneBanner.hidden = true;
    }, 1500);
  }

  function tileAt(x, y) {
    if (!state.map || y < 0 || y >= EXPLORE_H || x < 0 || x >= EXPLORE_W) return "wall";
    return state.map.tiles[y][x];
  }

  function objectAt(x, y) {
    return state.map.objects[keyOf(x, y)] || null;
  }

  function setObject(x, y, object) {
    const key = keyOf(x, y);
    if (object) state.map.objects[key] = object;
    else delete state.map.objects[key];
  }

  function makeItemName() {
    const materials = ["iron", "bronze", "ashwood", "moonstone", "blackglass", "silvered"];
    const forms = ["blade", "staff", "ward", "ring", "mail", "bow", "sigil"];
    const marks = ["keen", "patient", "sunlit", "grave-cold", "storm-bound", "red"];
    return `${marks[randInt(0, marks.length - 1)]} ${materials[randInt(0, materials.length - 1)]} ${forms[randInt(0, forms.length - 1)]}`;
  }

  function moveExplore(dirName) {
    if (state.mode !== "explore") return;
    const dir = dirMap[dirName];
    const nx = state.player.x + dir.x;
    const ny = state.player.y + dir.y;
    if (tileAt(nx, ny) !== "floor") {
      pushLog("Stone blocks the way.", "warn");
      render();
      return;
    }

    state.player.x = nx;
    state.player.y = ny;
    resolveExploreTile();
    render();
  }

  function resolveExploreTile() {
    const obj = objectAt(state.player.x, state.player.y);
    if (!obj) {
      if (random() < 0.025 + state.depth * 0.004) {
        startCombat("patrol");
      }
      return;
    }

    if (obj.type === "cache") {
      const gold = randInt(12, 28) + state.depth * 4;
      const item = makeItemName();
      state.gold += gold;
      state.inventory.push(item);
      state.inventory = state.inventory.slice(-12);
      if (random() < 0.5) state.elixirs += 1;
      if (random() < 0.55) state.supplies += 1;
      setObject(state.player.x, state.player.y, null);
      pushLog(`A sealed cache yields ${gold} gold and a ${item}.`, "good");
      showBanner("Cache opened");
    }

    if (obj.type === "shrine") {
      state.party.forEach((member) => {
        if (member.hp > 0) {
          member.hp = Math.min(member.maxHp, member.hp + 9 + state.depth * 2);
          member.mana = Math.min(member.maxMana, member.mana + 5);
        }
      });
      setObject(state.player.x, state.player.y, null);
      pushLog("An old shrine steadies the company.", "good");
      showBanner("Shrine restored");
    }

    if (obj.type === "encounter") {
      setObject(state.player.x, state.player.y, null);
      startCombat("guard");
    }

    if (obj.type === "stairs") {
      pushLog("A stairway sinks into a colder dark.", "");
      showBanner("Stairs found");
    }

    if (obj.type === "town") {
      state.party.forEach((member) => {
        if (member.hp > 0) {
          member.hp = Math.min(member.maxHp, member.hp + 16);
          member.mana = Math.min(member.maxMana, member.mana + 10);
        }
      });
      pushLog("Nedly opens its lamps to the company.", "good");
      showBanner("Nedly");
    }
  }

  function canDescend() {
    const obj = objectAt(state.player.x, state.player.y);
    return state.mode === "explore" && obj && obj.type === "stairs";
  }

  function descend() {
    if (!canDescend()) {
      pushLog("No stairway lies underfoot.", "warn");
      render();
      return;
    }
    state.depth += 1;
    state.supplies += 2;
    const map = generateMap(state.depth);
    state.map = map;
    state.player = { x: map.start.x, y: map.start.y };
    state.party.forEach((member) => {
      if (member.hp > 0) member.mana = Math.min(member.maxMana, member.mana + 3);
    });
    pushLog(`The company descends to depth ${state.depth}.`, "good");
    showBanner(`Depth ${state.depth}`);
    render();
  }

  function camp() {
    if (state.mode !== "explore") return;
    if (state.supplies <= 0) {
      pushLog("The packs hold no supplies.", "warn");
      render();
      return;
    }
    state.supplies -= 1;
    state.party.forEach((member) => {
      if (member.hp > 0) {
        member.hp = Math.min(member.maxHp, member.hp + 13);
        member.mana = Math.min(member.maxMana, member.mana + 8);
      }
    });
    pushLog("A guarded campfire buys a little strength.", "good");
    render();
  }

  function enemyTemplate(kind, depth) {
    const table = {
      sentry: { name: "Fallen Sentry", hp: 18, pow: 6, def: 2, agi: 4, range: 1, color: "#9b6b45" },
      adept: { name: "Dust Adept", hp: 15, pow: 7, def: 1, agi: 6, range: 3, color: "#7b8c99" },
      husk: { name: "Iron Husk", hp: 28, pow: 8, def: 5, agi: 2, range: 1, color: "#8d9387" },
      reaver: { name: "Grave Reaver", hp: 22, pow: 9, def: 3, agi: 5, range: 1, color: "#a34d52" }
    };
    const base = table[kind];
    return {
      id: `${kind}-${Math.random().toString(16).slice(2)}`,
      side: "enemy",
      name: base.name,
      role: kind,
      hp: base.hp + depth * 4,
      maxHp: base.hp + depth * 4,
      mana: 0,
      maxMana: 0,
      pow: base.pow + Math.floor(depth * 1.4),
      def: base.def + Math.floor(depth / 2),
      agi: base.agi,
      range: base.range,
      color: base.color,
      x: 0,
      y: 0,
      status: {}
    };
  }

  function startCombat(style) {
    const partyUnits = alive(state.party).map((member, index) => ({
      ...member,
      side: "party",
      x: index < 4 ? 1 : 2,
      y: index < 4 ? index * 2 : (index - 4) * 2 + 1,
      status: {}
    }));

    const enemyKinds = ["sentry", "adept", "husk", "reaver"];
    const count = clamp(3 + Math.floor(state.depth / 2) + (style === "patrol" ? 0 : 1), 3, 7);
    const enemies = [];
    for (let i = 0; i < count; i += 1) {
      const enemy = enemyTemplate(enemyKinds[randInt(0, enemyKinds.length - 1)], state.depth);
      enemy.x = COMBAT_W - 2 - (i % 2);
      enemy.y = i % COMBAT_H;
      enemies.push(enemy);
    }

    const obstacles = [];
    for (let i = 0; i < 7; i += 1) {
      const spot = { x: randInt(4, 8), y: randInt(1, COMBAT_H - 2) };
      if (!obstacles.some((o) => o.x === spot.x && o.y === spot.y)) obstacles.push(spot);
    }

    state.mode = "combat";
    state.combat = {
      round: 1,
      units: [...partyUnits, ...enemies],
      activeId: null,
      actionMode: "attack",
      moved: false,
      acted: false,
      obstacles
    };
    pushLog("Steel answers in the dark.", "warn");
    showBanner("Encounter");
    nextTurn();
  }

  function syncPartyFromCombat() {
    if (!state.combat) return;
    state.party = state.party.map((member) => {
      const combatUnit = state.combat.units.find((unit) => unit.id === member.id);
      if (!combatUnit) return member;
      return {
        ...member,
        hp: clamp(combatUnit.hp, 0, member.maxHp),
        mana: clamp(combatUnit.mana, 0, member.maxMana),
        skills: combatUnit.skills || member.skills
      };
    });
  }

  function currentUnit() {
    if (!state.combat) return null;
    return state.combat.units.find((unit) => unit.id === state.combat.activeId) || null;
  }

  function partyUnitById(id) {
    return state.party.find((member) => member.id === id);
  }

  function nextTurn() {
    if (!state.combat) return;
    clearTimeout(aiTimer);
    const partyAlive = state.combat.units.some((unit) => unit.side === "party" && unit.hp > 0);
    const enemiesAlive = state.combat.units.some((unit) => unit.side === "enemy" && unit.hp > 0);

    if (!partyAlive) {
      syncPartyFromCombat();
      state.mode = "defeat";
      state.combat = null;
      pushLog("The company falls. A new oath may yet be sworn.", "warn");
      showBanner("Company defeated");
      render();
      return;
    }

    if (!enemiesAlive) {
      finishCombat();
      return;
    }

    const living = state.combat.units
      .filter((unit) => unit.hp > 0)
      .sort((a, b) => b.agi - a.agi || a.name.localeCompare(b.name));

    let index = living.findIndex((unit) => unit.id === state.combat.activeId);
    if (index === -1 || index >= living.length - 1) {
      index = 0;
      if (state.combat.activeId !== null) state.combat.round += 1;
    } else {
      index += 1;
    }

    const active = living[index];
    active.status.guarding = false;
    state.combat.activeId = active.id;
    state.combat.actionMode = active.side === "party" ? "attack" : "thinking";
    state.combat.moved = false;
    state.combat.acted = false;
    render();

    if (active.side === "enemy") {
      aiTimer = setTimeout(() => {
        enemyAct(active.id);
      }, 360);
    }
  }

  function finishCombat() {
    const gold = randInt(18, 38) + state.depth * 6;
    syncPartyFromCombat();
    state.gold += gold;
    if (random() < 0.6) state.elixirs += 1;
    state.mode = "explore";
    state.combat = null;
    pushLog(`The field is won. ${gold} gold recovered.`, "good");
    showBanner("Victory");
    render();
  }

  function isObstacle(x, y) {
    return state.combat.obstacles.some((spot) => spot.x === x && spot.y === y);
  }

  function unitAtCombat(x, y) {
    return state.combat.units.find((unit) => unit.hp > 0 && unit.x === x && unit.y === y) || null;
  }

  function inCombatBounds(x, y) {
    return x >= 0 && x < COMBAT_W && y >= 0 && y < COMBAT_H;
  }

  function canStandAt(x, y) {
    return inCombatBounds(x, y) && !isObstacle(x, y) && !unitAtCombat(x, y);
  }

  function moveActiveTo(x, y) {
    const active = currentUnit();
    if (!active || active.side !== "party" || state.combat.moved) return;
    if (!canStandAt(x, y)) return;
    if (distance(active, { x, y }) > 3) {
      pushLog("That move is too far.", "warn");
      render();
      return;
    }
    active.x = x;
    active.y = y;
    state.combat.moved = true;
    pushLog(`${active.name} takes position.`, "");
    render();
  }

  function skillValue(unit, key) {
    return unit.skills && unit.skills[key] ? unit.skills[key] : 1;
  }

  function improveSkill(unit, key, amount) {
    if (!unit || unit.side !== "party") return;
    if (!unit.skills) unit.skills = {};
    const before = skillValue(unit, key);
    unit.skills[key] = clamp(before + amount, 1, 99);
    const base = partyUnitById(unit.id);
    if (base) {
      if (!base.skills) base.skills = {};
      base.skills[key] = unit.skills[key];
    }
    if (before < 15 && unit.skills[key] >= 15) pushLog(`${unit.name} is no longer lousy at ${key}.`, "good");
    if (before < 30 && unit.skills[key] >= 30) pushLog(`${unit.name} becomes steady at ${key}.`, "good");
  }

  function attack(attacker, target) {
    if (!attacker || !target || target.hp <= 0) return false;
    if (distance(attacker, target) > attacker.range) {
      pushLog(`${target.name} is out of reach.`, "warn");
      render();
      return false;
    }
    const guard = target.status && target.status.guarding ? 3 : 0;
    const skillKey = attacker.range > 1 ? "missile" : "arms";
    const skillBonus = Math.floor(skillValue(attacker, skillKey) / 5);
    const roll = randInt(0, 5);
    const damage = Math.max(1, attacker.pow + roll + skillBonus - target.def - guard);
    target.hp = Math.max(0, target.hp - damage);
    pushLog(`${attacker.name} hits ${target.name} for ${damage}.`, target.hp <= 0 ? "good" : "");
    improveSkill(attacker, skillKey, 1);
    if (target.hp <= 0) pushLog(`${target.name} is down.`, "good");
    return true;
  }

  function castSpell(caster, target) {
    if (!caster || !target || caster.side !== "party") return false;
    const base = partyUnitById(caster.id);
    if (!base || !base.spell) {
      pushLog(`${caster.name} knows no battle spell.`, "warn");
      render();
      return false;
    }

    if (base.spell === "mend") {
      if (target.side !== "party") {
        pushLog("Mend needs an ally.", "warn");
        render();
        return false;
      }
      if (caster.mana < 5) {
        pushLog(`${caster.name} lacks mana.`, "warn");
        render();
        return false;
      }
      const heal = 10 + Math.floor(skillValue(caster, "mend") / 2);
      target.hp = Math.min(target.maxHp, target.hp + heal);
      caster.mana -= 5;
      improveSkill(caster, "mend", 2);
      pushLog(`${caster.name} mends ${target.name} for ${heal}.`, "good");
      return true;
    }

    if (target.side !== "enemy") {
      pushLog("That spell needs an enemy mark.", "warn");
      render();
      return false;
    }
    if (distance(caster, target) > 4) {
      pushLog(`${target.name} is beyond the sigil.`, "warn");
      render();
      return false;
    }

    if (base.spell === "ember") {
      if (caster.mana < 6) {
        pushLog(`${caster.name} lacks mana.`, "warn");
        render();
        return false;
      }
      const damage = 10 + Math.floor(skillValue(caster, "ember") / 2) + randInt(0, 5);
      caster.mana -= 6;
      target.hp = Math.max(0, target.hp - damage);
      improveSkill(caster, "ember", 2);
      pushLog(`${caster.name} casts ember for ${damage}.`, target.hp <= 0 ? "good" : "");
      if (target.hp <= 0) pushLog(`${target.name} is down.`, "good");
      return true;
    }

    if (base.spell === "hex") {
      if (caster.mana < 5) {
        pushLog(`${caster.name} lacks mana.`, "warn");
        render();
        return false;
      }
      const damage = 6 + Math.floor(skillValue(caster, "hex") / 3) + randInt(0, 4);
      caster.mana -= 5;
      target.hp = Math.max(0, target.hp - damage);
      target.status.hexed = 2;
      improveSkill(caster, "hex", 2);
      pushLog(`${caster.name} hexes ${target.name}.`, target.hp <= 0 ? "good" : "");
      if (target.hp <= 0) pushLog(`${target.name} is down.`, "good");
      return true;
    }

    return false;
  }

  function activeUseElixir() {
    if (state.elixirs <= 0) {
      pushLog("No elixirs remain.", "warn");
      render();
      return;
    }

    if (state.mode === "combat") {
      const active = currentUnit();
      if (!active || active.side !== "party" || state.combat.acted) return;
      state.elixirs -= 1;
      active.hp = Math.min(active.maxHp, active.hp + 18);
      active.mana = Math.min(active.maxMana, active.mana + 6);
      state.combat.acted = true;
      pushLog(`${active.name} drinks an elixir.`, "good");
      endActiveTurn();
      return;
    }

    const target = alive(state.party).sort((a, b) => a.hp / a.maxHp - b.hp / b.maxHp)[0];
    if (!target) return;
    state.elixirs -= 1;
    target.hp = Math.min(target.maxHp, target.hp + 18);
    target.mana = Math.min(target.maxMana, target.mana + 6);
    pushLog(`${target.name} drinks an elixir.`, "good");
    render();
  }

  function guardActive() {
    const active = currentUnit();
    if (!active || active.side !== "party") return;
    active.status.guarding = true;
    improveSkill(active, "guard", 1);
    pushLog(`${active.name} guards the line.`, "");
    endActiveTurn();
  }

  function endActiveTurn() {
    if (state.mode !== "combat") return;
    syncPartyFromCombat();
    nextTurn();
  }

  function enemyAct(enemyId) {
    if (!state.combat || state.mode !== "combat") return;
    const enemy = state.combat.units.find((unit) => unit.id === enemyId && unit.hp > 0);
    if (!enemy) {
      nextTurn();
      return;
    }

    let target = alive(state.combat.units.filter((unit) => unit.side === "party"))
      .sort((a, b) => distance(enemy, a) - distance(enemy, b) || a.hp - b.hp)[0];

    if (!target) {
      nextTurn();
      return;
    }

    if (distance(enemy, target) > enemy.range) {
      const options = [
        { x: enemy.x + Math.sign(target.x - enemy.x), y: enemy.y },
        { x: enemy.x, y: enemy.y + Math.sign(target.y - enemy.y) },
        { x: enemy.x - Math.sign(target.x - enemy.x), y: enemy.y },
        { x: enemy.x, y: enemy.y - Math.sign(target.y - enemy.y) }
      ];
      const step = options.find((spot) => canStandAt(spot.x, spot.y));
      if (step) {
        enemy.x = step.x;
        enemy.y = step.y;
      }
    }

    target = alive(state.combat.units.filter((unit) => unit.side === "party"))
      .sort((a, b) => distance(enemy, a) - distance(enemy, b) || a.hp - b.hp)[0];

    if (target && distance(enemy, target) <= enemy.range) {
      const oldPow = enemy.pow;
      if (enemy.status.hexed) enemy.pow = Math.max(1, enemy.pow - 3);
      attack(enemy, target);
      enemy.pow = oldPow;
      if (enemy.status.hexed) {
        enemy.status.hexed -= 1;
        if (enemy.status.hexed <= 0) delete enemy.status.hexed;
      }
    } else {
      pushLog(`${enemy.name} advances.`, "");
    }

    syncPartyFromCombat();
    render();
    aiTimer = setTimeout(nextTurn, 300);
  }

  function handleCombatClick(x, y) {
    if (!state.combat) return;
    const active = currentUnit();
    if (!active || active.side !== "party") return;

    const gx = Math.floor((x - COMBAT_LEFT) / COMBAT_CELL);
    const gy = Math.floor((y - COMBAT_TOP) / COMBAT_CELL);
    if (!inCombatBounds(gx, gy)) return;

    const target = unitAtCombat(gx, gy);

    if (state.combat.actionMode === "move") {
      moveActiveTo(gx, gy);
      return;
    }

    if (!target) return;

    if (state.combat.actionMode === "cast") {
      if (state.combat.acted) return;
      if (castSpell(active, target)) {
        state.combat.acted = true;
        endActiveTurn();
      }
      return;
    }

    if (state.combat.actionMode === "attack") {
      if (state.combat.acted || target.side !== "enemy") return;
      if (attack(active, target)) {
        state.combat.acted = true;
        endActiveTurn();
      }
    }
  }

  function handleExploreClick(x, y) {
    const gx = Math.floor(x / TILE);
    const gy = Math.floor(y / TILE);
    const dx = gx - state.player.x;
    const dy = gy - state.player.y;
    if (Math.abs(dx) + Math.abs(dy) !== 1) return;
    if (dx === 1) moveExplore("east");
    if (dx === -1) moveExplore("west");
    if (dy === 1) moveExplore("south");
    if (dy === -1) moveExplore("north");
  }

  function saveGame() {
    localStorage.setItem(SAVE_KEY, JSON.stringify(state));
    pushLog("The current oath is saved.", "good");
    render();
  }

  function loadGame() {
    const raw = localStorage.getItem(SAVE_KEY);
    if (!raw) {
      pushLog("No saved oath is present.", "warn");
      render();
      return;
    }
    try {
      state = JSON.parse(raw);
      pushLog("The saved oath is restored.", "good");
      render();
    } catch (error) {
      pushLog("The save could not be read.", "warn");
      render();
    }
  }

  function render() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    if (state.mode === "combat") drawCombat();
    else drawExplore();
    updatePanels();
  }

  function drawExplore() {
    ctx.fillStyle = "#0a0d0f";
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    for (let y = 0; y < EXPLORE_H; y += 1) {
      for (let x = 0; x < EXPLORE_W; x += 1) {
        const visible = distance({ x, y }, state.player) <= 7;
        if (tileAt(x, y) === "wall") {
          ctx.fillStyle = visible ? "#252d2d" : "#101416";
          ctx.fillRect(x * TILE, y * TILE, TILE, TILE);
          if (visible) {
            ctx.fillStyle = "rgba(255,255,255,0.04)";
            ctx.fillRect(x * TILE + 3, y * TILE + 3, TILE - 6, 3);
          }
        } else {
          ctx.fillStyle = visible ? "#3a3329" : "#171511";
          ctx.fillRect(x * TILE, y * TILE, TILE, TILE);
          if (visible) {
            ctx.fillStyle = randomStoneColor(x, y);
            ctx.fillRect(x * TILE + 2, y * TILE + 2, TILE - 4, TILE - 4);
          }
        }
        ctx.strokeStyle = "rgba(0,0,0,0.2)";
        ctx.strokeRect(x * TILE, y * TILE, TILE, TILE);
      }
    }

    Object.keys(state.map.objects).forEach((key) => {
      const [x, y] = key.split(",").map(Number);
      if (distance({ x, y }, state.player) > 7) return;
      drawObject(x, y, state.map.objects[key].type);
    });

    drawPartyMarker(state.player.x, state.player.y);
    drawExploreVignette();
  }

  function randomStoneColor(x, y) {
    const n = (x * 17 + y * 31 + state.depth * 13) % 5;
    return ["#40372b", "#3a3329", "#493b2a", "#34413b", "#2d3839"][n];
  }

  function drawObject(x, y, type) {
    const px = x * TILE;
    const py = y * TILE;
    const cx = px + TILE / 2;
    const cy = py + TILE / 2;
    ctx.save();
    if (type === "cache") {
      ctx.fillStyle = "#9a6a3e";
      ctx.fillRect(px + 7, py + 11, 18, 14);
      ctx.fillStyle = "#d7a84e";
      ctx.fillRect(px + 9, py + 13, 14, 3);
    }
    if (type === "shrine") {
      ctx.fillStyle = "#58b7a5";
      ctx.beginPath();
      ctx.arc(cx, cy, 9, 0, Math.PI * 2);
      ctx.fill();
      ctx.fillStyle = "#f3ead7";
      ctx.fillRect(cx - 2, cy - 12, 4, 24);
      ctx.fillRect(cx - 8, cy - 2, 16, 4);
    }
    if (type === "encounter") {
      ctx.fillStyle = "#b94b56";
      ctx.beginPath();
      ctx.moveTo(cx, cy - 11);
      ctx.lineTo(cx + 11, cy + 10);
      ctx.lineTo(cx - 11, cy + 10);
      ctx.closePath();
      ctx.fill();
    }
    if (type === "stairs") {
      ctx.fillStyle = "#cfc5ad";
      for (let i = 0; i < 5; i += 1) {
        ctx.fillRect(px + 7 + i * 2, py + 8 + i * 4, 18, 3);
      }
    }
    if (type === "camp") {
      ctx.fillStyle = "#c65c3b";
      ctx.beginPath();
      ctx.moveTo(cx, cy - 10);
      ctx.lineTo(cx + 7, cy + 7);
      ctx.lineTo(cx - 7, cy + 7);
      ctx.closePath();
      ctx.fill();
      ctx.fillStyle = "#e6c36f";
      ctx.fillRect(cx - 2, cy - 2, 4, 10);
    }
    if (type === "town") {
      ctx.fillStyle = "#7f9d5b";
      ctx.fillRect(px + 7, py + 14, 18, 12);
      ctx.fillStyle = "#d7a84e";
      ctx.beginPath();
      ctx.moveTo(px + 5, py + 15);
      ctx.lineTo(cx, py + 6);
      ctx.lineTo(px + 27, py + 15);
      ctx.closePath();
      ctx.fill();
      ctx.fillStyle = "#111619";
      ctx.fillRect(px + 14, py + 18, 4, 8);
    }
    ctx.restore();
  }

  function drawPartyMarker(x, y) {
    const px = x * TILE;
    const py = y * TILE;
    ctx.save();
    ctx.fillStyle = "#101619";
    ctx.beginPath();
    ctx.arc(px + 16, py + 17, 14, 0, Math.PI * 2);
    ctx.fill();
    ctx.strokeStyle = "#e6c36f";
    ctx.lineWidth = 2;
    ctx.stroke();
    ctx.fillStyle = "#58b7a5";
    ctx.fillRect(px + 10, py + 9, 12, 16);
    ctx.fillStyle = "#f3ead7";
    ctx.fillRect(px + 13, py + 5, 6, 6);
    ctx.restore();
  }

  function drawExploreVignette() {
    const gradient = ctx.createRadialGradient(
      state.player.x * TILE + 16,
      state.player.y * TILE + 16,
      90,
      state.player.x * TILE + 16,
      state.player.y * TILE + 16,
      360
    );
    gradient.addColorStop(0, "rgba(0,0,0,0)");
    gradient.addColorStop(1, "rgba(0,0,0,0.72)");
    ctx.fillStyle = gradient;
    ctx.fillRect(0, 0, canvas.width, canvas.height);
  }

  function drawCombat() {
    ctx.fillStyle = "#101316";
    ctx.fillRect(0, 0, canvas.width, canvas.height);
    drawCombatBackdrop();

    for (let y = 0; y < COMBAT_H; y += 1) {
      for (let x = 0; x < COMBAT_W; x += 1) {
        const px = COMBAT_LEFT + x * COMBAT_CELL;
        const py = COMBAT_TOP + y * COMBAT_CELL;
        ctx.fillStyle = (x + y) % 2 === 0 ? "#302b24" : "#383027";
        ctx.fillRect(px, py, COMBAT_CELL, COMBAT_CELL);
        ctx.strokeStyle = "#171b1d";
        ctx.strokeRect(px, py, COMBAT_CELL, COMBAT_CELL);

        if (isObstacle(x, y)) {
          ctx.fillStyle = "#55605b";
          ctx.fillRect(px + 12, py + 16, COMBAT_CELL - 24, COMBAT_CELL - 22);
          ctx.fillStyle = "rgba(0,0,0,0.22)";
          ctx.fillRect(px + 17, py + 22, COMBAT_CELL - 30, 5);
        }
      }
    }

    const active = currentUnit();
    if (active && active.side === "party") {
      drawReach(active);
    }

    state.combat.units.forEach(drawCombatUnit);
  }

  function drawCombatBackdrop() {
    ctx.fillStyle = "#1d2525";
    ctx.fillRect(80, 40, 820, 540);
    ctx.fillStyle = "rgba(88,183,165,0.13)";
    ctx.fillRect(85, 45, 180, 530);
    ctx.fillStyle = "rgba(198,92,59,0.13)";
    ctx.fillRect(700, 45, 195, 530);
  }

  function drawReach(unit) {
    if (state.combat.actionMode !== "move" || state.combat.moved) return;
    ctx.save();
    ctx.fillStyle = "rgba(88, 183, 165, 0.24)";
    for (let y = 0; y < COMBAT_H; y += 1) {
      for (let x = 0; x < COMBAT_W; x += 1) {
        if (distance(unit, { x, y }) <= 3 && canStandAt(x, y)) {
          ctx.fillRect(COMBAT_LEFT + x * COMBAT_CELL + 4, COMBAT_TOP + y * COMBAT_CELL + 4, COMBAT_CELL - 8, COMBAT_CELL - 8);
        }
      }
    }
    ctx.restore();
  }

  function drawCombatUnit(unit) {
    const px = COMBAT_LEFT + unit.x * COMBAT_CELL;
    const py = COMBAT_TOP + unit.y * COMBAT_CELL;
    const active = state.combat.activeId === unit.id;
    ctx.save();
    ctx.globalAlpha = unit.hp > 0 ? 1 : 0.35;
    if (active) {
      ctx.strokeStyle = "#e6c36f";
      ctx.lineWidth = 4;
      ctx.strokeRect(px + 5, py + 5, COMBAT_CELL - 10, COMBAT_CELL - 10);
    }
    ctx.fillStyle = unit.side === "party" ? "#58b7a5" : unit.color || "#b94b56";
    ctx.beginPath();
    ctx.arc(px + COMBAT_CELL / 2, py + 24, 15, 0, Math.PI * 2);
    ctx.fill();
    ctx.fillStyle = unit.side === "party" ? "#1b2728" : "#211819";
    ctx.fillRect(px + 17, py + 34, COMBAT_CELL - 34, 14);
    ctx.fillStyle = "#f3ead7";
    ctx.font = "700 12px Segoe UI, Arial";
    ctx.textAlign = "center";
    ctx.fillText(unit.name.slice(0, 2).toUpperCase(), px + COMBAT_CELL / 2, py + 28);

    ctx.fillStyle = "#111619";
    ctx.fillRect(px + 9, py + COMBAT_CELL - 10, COMBAT_CELL - 18, 5);
    ctx.fillStyle = unit.side === "party" ? "#c65c3b" : "#b94b56";
    ctx.fillRect(px + 9, py + COMBAT_CELL - 10, (COMBAT_CELL - 18) * (unit.hp / unit.maxHp), 5);
    ctx.restore();
  }

  function updatePanels() {
    depthValue.textContent = String(state.depth);
    goldValue.textContent = String(state.gold);
    suppliesValue.textContent = String(state.supplies);
    elixirValue.textContent = String(state.elixirs);
    modeValue.textContent = state.mode === "combat" ? "Combat" : state.mode === "defeat" ? "Defeat" : "Explore";

    const active = currentUnit();
    turnBadge.textContent = active ? `${active.name}` : state.mode === "defeat" ? "Fallen" : "Ready";
    roundBadge.textContent = state.combat ? `Round ${state.combat.round}` : "Round 0";
    actionBadge.textContent = state.combat ? state.combat.actionMode : "Explore";

    partyList.innerHTML = state.party.map((member) => unitCard(member, active && active.id === member.id)).join("");

    const enemies = state.combat ? state.combat.units.filter((unit) => unit.side === "enemy") : [];
    enemyList.innerHTML = enemies.length
      ? enemies.map((enemy) => enemyCard(enemy, active && active.id === enemy.id)).join("")
      : `<div class="log-entry">No active opposition.</div>`;

    logList.innerHTML = state.log
      .map((entry) => `<div class="log-entry ${entry.tone}">${escapeHtml(entry.text)}</div>`)
      .join("");

    updateButtons();
  }

  function unitCard(member, active) {
    const down = member.hp <= 0;
    const leadSkill = bestSkill(member);
    const attr = member.attrs || { str: 0, int: 0, dex: 0, health: 0 };
    return `
      <article class="member-card ${active ? "active" : ""} ${down ? "down" : ""}">
        <div class="unit-title">
          <strong>${escapeHtml(member.name)}</strong>
          <span>${escapeHtml(member.role)} / ${leadSkill.label} ${leadSkill.value}</span>
        </div>
        <div class="meter-stack">
          <div class="meter hp"><span style="width:${pct(member.hp, member.maxHp)}%"></span></div>
          <div class="meter mana"><span style="width:${pct(member.mana, member.maxMana)}%"></span></div>
          <div class="meter skill"><span style="width:${pct(leadSkill.value, 50)}%"></span></div>
          <div class="unit-stats"><span>S ${attr.str} I ${attr.int}</span><span>D ${attr.dex} H ${attr.health}</span></div>
          <div class="unit-stats"><span>HP ${member.hp}/${member.maxHp}</span><span>MP ${member.mana}/${member.maxMana}</span></div>
        </div>
      </article>
    `;
  }

  function bestSkill(member) {
    const labels = {
      arms: "arms",
      missile: "missile",
      mend: "mend",
      ember: "ember",
      hex: "hex",
      guard: "guard"
    };
    const skills = member.skills || {};
    const key = Object.keys(labels).sort((a, b) => (skills[b] || 0) - (skills[a] || 0))[0] || "arms";
    return { label: labels[key], value: skills[key] || 1 };
  }

  function enemyCard(enemy, active) {
    const down = enemy.hp <= 0;
    return `
      <article class="enemy-card ${active ? "active" : ""} ${down ? "down" : ""}">
        <div class="unit-title">
          <strong>${escapeHtml(enemy.name)}</strong>
          <span>${escapeHtml(enemy.role)}</span>
        </div>
        <div class="meter-stack">
          <div class="meter hp"><span style="width:${pct(enemy.hp, enemy.maxHp)}%"></span></div>
          <div class="unit-stats"><span>HP ${enemy.hp}/${enemy.maxHp}</span><span>R ${enemy.range}</span></div>
        </div>
      </article>
    `;
  }

  function updateButtons() {
    const combat = state.mode === "combat";
    const explore = state.mode === "explore";
    const active = currentUnit();
    const playerTurn = combat && active && active.side === "party";
    document.querySelectorAll("[data-dir]").forEach((button) => {
      button.disabled = !explore;
    });
    buttons.camp.disabled = !explore;
    buttons.descend.disabled = !canDescend();
    buttons.potion.disabled = state.elixirs <= 0 || state.mode === "defeat" || (combat && !playerTurn);
    buttons.move.disabled = !playerTurn || state.combat.moved;
    buttons.attack.disabled = !playerTurn || state.combat.acted;
    buttons.cast.disabled = !playerTurn || state.combat.acted || !partyUnitById(active.id).spell;
    buttons.guard.disabled = !playerTurn;
    buttons.wait.disabled = !playerTurn;
  }

  function escapeHtml(text) {
    return String(text)
      .replaceAll("&", "&amp;")
      .replaceAll("<", "&lt;")
      .replaceAll(">", "&gt;")
      .replaceAll('"', "&quot;");
  }

  function setActionMode(mode) {
    if (state.mode !== "combat") return;
    const active = currentUnit();
    if (!active || active.side !== "party") return;
    state.combat.actionMode = mode;
    render();
  }

  canvas.addEventListener("click", (event) => {
    const rect = canvas.getBoundingClientRect();
    const x = (event.clientX - rect.left) * (canvas.width / rect.width);
    const y = (event.clientY - rect.top) * (canvas.height / rect.height);
    if (state.mode === "combat") handleCombatClick(x, y);
    else if (state.mode === "explore") handleExploreClick(x, y);
  });

  document.querySelectorAll("[data-dir]").forEach((button) => {
    button.addEventListener("click", () => moveExplore(button.dataset.dir));
  });

  buttons.camp.addEventListener("click", camp);
  buttons.descend.addEventListener("click", descend);
  buttons.potion.addEventListener("click", activeUseElixir);
  buttons.move.addEventListener("click", () => setActionMode("move"));
  buttons.attack.addEventListener("click", () => setActionMode("attack"));
  buttons.cast.addEventListener("click", () => setActionMode("cast"));
  buttons.guard.addEventListener("click", guardActive);
  buttons.wait.addEventListener("click", endActiveTurn);
  buttons.save.addEventListener("click", saveGame);
  buttons.load.addEventListener("click", loadGame);
  buttons.newGame.addEventListener("click", () => {
    if (confirm("Start a new company?")) newGame();
  });

  window.addEventListener("keydown", (event) => {
    if (event.target && ["INPUT", "TEXTAREA", "BUTTON"].includes(event.target.tagName)) return;
    if (event.key === "F1") {
      event.preventDefault();
      const help = state.mode === "combat"
        ? "F1: choose Move, Attack, Cast, Guard, Elixir, or Wait; then click the grid."
        : "F1: travel with arrows or WASD, visit Nedly, open caches, and descend by stairs.";
      pushLog(help, "good");
      showBanner("Context help");
      render();
      return;
    }
    if (event.key === "ArrowUp" || event.key.toLowerCase() === "w") moveExplore("north");
    if (event.key === "ArrowDown" || event.key.toLowerCase() === "s") moveExplore("south");
    if (event.key === "ArrowLeft" || event.key.toLowerCase() === "a") moveExplore("west");
    if (event.key === "ArrowRight" || event.key.toLowerCase() === "d") moveExplore("east");
    if (event.key === "1") setActionMode("move");
    if (event.key === "2") setActionMode("attack");
    if (event.key === "3") setActionMode("cast");
    if (event.key === " ") endActiveTurn();
  });

  newGame();
}());
