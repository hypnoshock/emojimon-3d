// SPDX-License-Identifier: MIT
pragma solidity >=0.8.0;

/* Autogenerated file. Do not edit manually. */

import { IStore } from "@latticexyz/store/src/IStore.sol";

import { IWorldCore } from "@latticexyz/world/src/interfaces/IWorldCore.sol";

import { IEncounterSystem } from "./IEncounterSystem.sol";
import { IMapSystem } from "./IMapSystem.sol";

/**
 * The IWorld interface includes all systems dynamically added to the World
 * during the deploy process.
 */
interface IWorld is IStore, IWorldCore, IEncounterSystem, IMapSystem {

}
