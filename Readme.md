<p align="center">
  <a href="https://github.com/BlackTasty/DungeonMapEditor">
    <img alt="Dungeon Map Editor" width="256" heigth="256" src="https://via.placeholder.com/256">
  </a>
</p>

# Dungeon Map Editor (DME)

Tired of drawing dungeon maps by yourself? Maps you find online just don't fit your style? Then you'll love this program! Create custom tiles and objects and create maps faster than before!

## Status

![GitHub issues](https://img.shields.io/github/issues/BlackTasty/DungeonMapEditor)
![GitHub last commit](https://img.shields.io/github/last-commit/BlackTasty/DungeonMapEditor)

![GitHub release](https://img.shields.io/github/release/BlackTasty/DungeonMapEditor)
![GitHub Release Date](https://img.shields.io/github/release-date/BlackTasty/DungeonMapEditor)

![GitHub (pre-)release](https://img.shields.io/github/release/BlackTasty/DungeonMapEditor/all.svg?style=flat-square&label=pre-release)
![Github Pre-Releases](https://img.shields.io/github/downloads-pre/BlackTasty/DungeonMapEditor/latest/total.svg?style=flat-square&colorB=f57b40)
![GitHub (Pre-)Release Date](https://img.shields.io/github/release-date-pre/BlackTasty/DungeonMapEditor.svg?style=flat-square&label=pre-release%20date&colorB=f57b40)

## First stable release ToDo

- [x] Create and manage collections, containing map tiles and placeable objects
- [x] Create and manage projects. Projects contain your precious rooms and floors
- [x] Export maps as image (if you plan to use your map on a website like roll20)
- [x] Export maps as pdf
- [x] Import and export of collections
- [ ] Settings
- [x] Draw rooms
- [ ] Context menus on various controls
- [x] Auto-Updater

## Contributing

If you want to introduce new user controls and/or add new controls to existing ones, please follow the MVVM pattern.

Also please follow these guidelines:
- If you wish to contribute create a fork of this repo, after you are done make a merge request and describe in a few sentences what you have changed.
- Create Viewmodels inside the namespace "ViewModel" and append "ViewModel" at the end of the class name. (For example "ProjectOverview**ViewModel**")

## Screenshots

### Home Screen
![Home Screen](https://i.imgur.com/8fqpMlf.png "Home Screen")

### Collection manager
![Collection manager](https://i.imgur.com/wv2In89.png "Collection manager")

### Project overview
![Project overview](https://i.imgur.com/4raLL5O.png "Project overview")

### Room editor (Draw-a-room feature)
![Room editor](https://i.imgur.com/QiS8Efn.gif "Room editor")

### Floor editor
![Floor editor](https://i.imgur.com/GGFrtj1.png "Floor editor")

### Document layout
![Document layout](https://i.imgur.com/8MhHI9t.png "Document layout")
