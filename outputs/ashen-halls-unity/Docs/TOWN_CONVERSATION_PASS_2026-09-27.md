# Midgaard conversation pass — 2026-09-27

Orren, Kate, Lute, Borin, Tessa, and Maud now have authored greetings that remember campaign progress or their completed services. The content remains local and deterministic; dialogue does not call an AI service or add save fields.

- Orren recognizes the cistern den, kobold king, Gloam Warden, Glass Road, and Crownroad Marshal victories. His road gossip follows those milestones and directs later chapter reports to Scout Yara. Defeating the Marshal does not skip the Salt Cistern Gate survey. Completed debriefs stop requesting the same report.
- Orren, Borin, Tessa, and Maud each have a fourth, optional personal topic. The stories give them histories beyond their shop and tutorial roles: provision carts, a mother's horseshoeing lessons, caravan weapon fitting, and a troublesome enchanted lamp.
- Kate and Lute remember their shared introductory order and stop advertising a bundle the party already bought. Their existing cistern food advice now carries distinct personal histories and a connection between the two vendors.
- Borin and Tessa recognize their previous equipment sales. Maud recognizes earlier enchantment work. All available services retain exact prices, explicit purchase review, and the same resource effects.
- Older saves with a completed sewer reward still receive the town's recognition even if they lack the individual chamber victory flag.

## Verification

`TownConversationSmoke.RunOrThrow` covers victory precedence, early chapter flags that must not imply victory, legacy completion flags, shared purchase memory, exact service prices and duration, unchanged persistent state, and the correct late-campaign contact. Its greeting matrix covers all seven story stages, both purchase states, six speakers, and gold values 0, 240, and `int.MaxValue`; every opening keeps its choices on the first dialogue page.

The smoke passed in a standalone C# harness using the actual catalog, story flags, paging rules, and smoke source, with simple runtime data and logging stubs. The game source assembly also compiled with zero diagnostics using Unity's compiler/reference configuration. The first editor source compile encountered only the other agents' not-yet-written quest smoke types; it was not a gameplay test.

`RuntimeBootSmoke` now selects the new personal topics, returns through response paging, and checks that the entire serialized game state is unchanged. Existing purchase, confirmation, exact item identity, repeat-purchase prevention, and enchantment expiry assertions remain. These runtime checks require the coordinated Unity run; the standalone harness does not validate their live UI behavior.

The integrated Full Unity audit now passes all 13 gates, including TownConversationSmoke and the production social-dialogue assertions in RuntimeBootSmoke. Evidence: QA/project-audit/20260927-185141-763517a2/unity.log.
