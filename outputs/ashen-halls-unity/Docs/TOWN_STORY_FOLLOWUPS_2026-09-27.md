# Town story follow-ups — 2026-09-27

Edda and Maud now offer a short personal conversation after their optional favors are settled. Their existing greetings already describe the chosen outcome; these topics show what it means to the speaker without repeating the reward or market explanation.

## Player flow

| Speaker | Existing route | New topic | Availability and response |
| --- | --- | --- | --- |
| Edda | About your companions... | How have you been? | After `RemembranceDelivered`; the private memorial recalls Jory beside the remembrance book, while the public account leads a guard to ask about Nella. |
| Maud | About the brass weigh-lamp... | Has this changed how you work? | After `HonestWeightBroken` or `HonestWeightRetuned`; breaking the charm prompts Maud to reconsider old clauses, while retuning it prompts her to show customers the working clause. |

Both response screens return to the favor's topics through the existing native Back to topics action. Edda's completed Back hint now says it returns to her other stories. Unfinished favors retain their existing hint.

Edda's completed menu has two choices. Maud has four with a tool still pending (`reward`, `account`, `work`, `back`) and three after it is claimed. Existing service, account, keepsake, and leave routes remain available. No root-menu button is added.

## Authored responses

- Edda, private: “I sat by the book and remembered Jory complaining about wet socks. For a little while, I could hear him clearly.”
- Edda, public: “A young guard asked what Nella was like. I told him she counted us twice before every trip. He stayed for the whole story.”
- Maud, broken: “I've been checking the old work on that shelf. Some of it would serve better with a clause crossed out. My younger self would be quite offended.”
- Maud, retuned: “I've started leaving the working clause where the customer can read it. They ask tiresome questions. Good. I should have invited those sooner.”

## Scope and validation

Runtime changes are limited to `AshenHallsGame.RemembranceQuest.cs` and `AshenHallsGame.HonestWeight.cs`. Existing story flags determine every branch. The topics have no reward, price, experience, quest progression, random roll, new save field, or read-once marker. Existing saves with completed favors gain the topic immediately; unfinished saves do not.

`AshenHalls.Editor.TownStoryFollowupSmoke.RunOrThrow()` is the integration entry; `.Run()` is the standalone editor entry. The coordinating chat registered it in the shared Full checks. We used an agreed exclusive Unity slot for the focused checks and then returned it for combined verification.

The regression exercises actual bound native dialogue buttons and return callbacks. It covers unavailable and direct-dispatch rejection before completion, both dispositions with pending and claimed rewards, actual SaveService roundtrips, all retained menu routes within the four-button limit, repeated reading with exact serialized campaign equality, and unchanged gameplay/Unity random state. Its inactive fixture explicitly releases its own root canvases and restores borrowed input ownership.

The focused licensed Unity regression passed in `QA/coordinated-followup-town-story-attempt3.log` (`Ash & Brimstone town story follow-up smoke passed.`). Source compilation also passed with 175 runtime and 74 Editor sources and zero diagnostics in `QA/source-compile/20260927-221443-8e946f74`.

The coordinated native Editor capture run passed in `QA/coordinated-followup-native-capture-attempt3.log` and produced 26 images in `QA/town-and-pack-followups/20260927-221948-385/`. The 20 story images cover Edda's private/public completed menu and response (eight images) and Maud's broken/retuned pending-reward menu, claimed-reward menu, and work response (12 images), each at 960×600 and 1280×720. Direct review found complete dialogue, correct portraits, all intended topics and reward access, and visible return/leave controls without clipping. The remaining six images verify the coordinated Pack warning change.

The capture helper uses production native screens with isolated fixture states and restores its borrowed UI/input state. It does not load or write the player's live save. Visual review files are alongside the captures. This evidence covers focused Editor behavior and layout; combined Full checks, connected player preview, and packaging remain owned by the coordinating chat. No new release is claimed by this follow-up.
