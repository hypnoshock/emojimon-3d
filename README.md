# emojimon-3d

A blockchain integrated Unity3D project utilising MUD created in a day as a submission to ETH Tokyo 2023

## MUD2 SANDBOX

This project is based on a commit in [MUD v2 Sabnbox](https://github.com/latticexyz/v2sandbox/commit/98461d5154eb4f9547d759db3df83b37fb481e0f), which contains the Emojimon project updated to MUD2 in the [holic/emojimon](https://github.com/latticexyz/v2sandbox/compare/holic/emojimon) branch.

### Getting started

- Press the button above to copy this branch to a new repository on your account. Alternatively just clone this repository.
- Run the following commands in the root folder of your project:
  1. Install all dependencies: `yarn`
  2. Initialize the auto-generated files: `yarn initialize`
  3. Initialize the PRIVATE_KEY environment variable with the default anvil private key: `echo "PRIVATE_KEY=0xac0974bec39a17e36ba4a6b4d238ff944bacb478cbed5efcae784d7bf4f2ff80" > packages/contracts/.env`
  4. Run the project locally: `yarn dev`

## Notes on adding Unity

This project adds an interface from the existing React frontend to an exported Unity WebGL build. So it's running React AND Unity.

### Signifcance

The MUD framework is getting widely used. While it is frontend agnostic, without an example of it working with a Unity frontend there's a barrier to some game development projects.

This project does not demonstarte a general way of using Unity with MUD but it shows one way do it and may be a starting point for a more re-usable Unity-MUD interface.

### Further work

The Unity project takes a json formatted state object and fills in equivelent c# strutures to be set the emojimon scene as set of 3D Asstets. Without the react component system, the state object would need to be created from the on-chain data / indexer.

The project does not work when running on any platform other than the Unity WebGL export. The game can be developed in the Editor but only with dummy data. Ideally a Unity interface could represent the on-chain state of MUD when running in Editor or non-webGL platforms like Mobile or Desktop. There are a few approaches here but generally there needs to be Unity-c# representation of the indexed MUD store data including code-generation of data structures.



