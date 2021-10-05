# Friend Locations

## Requirements

- [MelonLoader 0.4.x](https://melonwiki.xyz/)
- [VRChatUtilityKit](https://github.com/loukylor/VRC-Mods/tree/main/VRChatUtilityKit)

## Features

Adds a new UI page where you can see all your online friends you could join listed by instances. (Basically like the VRChat Website)
World Images and Player Names are clickable and points to the corresponding info page.

The button to access the menu is on the social page at the top of screen.

API Informations can be disabled in the MelonPreferences or with UIExpansionKit.

---

## VRChat API

To prevent malicious API traffic from this mod the API requests are limited to one request in 2-5 seconds. Also the mod checks the internal client cache for objects to minimize count online API requests.

---

### Menu Screenshot
![UI Screenshot](https://i.imgur.com/6uOOYVV.png)

---

## Credits

- Some of the Asset loading methods are inspired from [UIExpansionKit](https://github.com/knah/VRCMods/tree/master/UIExpansionKit) by [Knah](https://github.com/knah)

- BigMenu things and methods are inspired from [AskToPortal](https://github.com/loukylor/VRC-Mods/tree/main/AskToPortal) and [VRChatUtilityKit](https://github.com/loukylor/VRC-Mods/tree/main/VRChatUtilityKit) by [loukylor](https://github.com/loukylor)