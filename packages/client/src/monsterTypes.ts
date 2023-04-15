export enum MonsterType {
  Eagle = 1,
  Rat,
  Caterpillar,
}

type MonsterConfig = {
  name: string;
  emoji: string;
};

export const monsterTypes: Record<MonsterType, MonsterConfig> = {
  [MonsterType.Eagle]: {
    name: "Panda",
    emoji: "🐼",
  },
  [MonsterType.Rat]: {
    name: "Lion",
    emoji: "🦁",
  },
  [MonsterType.Caterpillar]: {
    name: "Pig",
    emoji: "🐷",
  },
};
