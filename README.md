# Friend Locations

## Requirements

- [MelonLoader 0.4.x](https://melonwiki.xyz/)
- [VRChatUtilityKit](https://github.com/loukylor/VRC-Mods/tree/main/VRChatUtilityKit)

## Features

Adds a new UI page where you can see all your online friends you could join listed by instances. (Basically like the VRChat Website)
- World Images and Player Names are clickable and points to the corresponding info page
- Shows Instance Type, Region and Publish State of the worlds
- UI Color can be auto adjusted to the rest of VRChat UI
- Borders can be showed around players to see them better
- API Informations are available

Most things can be enabled/disabled in the MelonPreferences or with UIExpansionKit.

The button to access the menu is on the social page at the top of screen.

---

## VRChat API

To prevent malicious API traffic from this mod the API requests are limited to one request in 2-5 seconds. Also the mod checks the internal client cache for objects to minimize count of online API requests.

---

### Menu Screenshot
![UI Screenshot](https://i.imgur.com/Vj4v11t.png)

---

## Credits

- Some of the Asset loading methods are inspired from [UIExpansionKit](https://github.com/knah/VRCMods/tree/master/UIExpansionKit) by [Knah](https://github.com/knah)

- BigMenu things and methods are inspired from [AskToPortal](https://github.com/loukylor/VRC-Mods/tree/main/AskToPortal) and [VRChatUtilityKit](https://github.com/loukylor/VRC-Mods/tree/main/VRChatUtilityKit) by [loukylor](https://github.com/loukylor)