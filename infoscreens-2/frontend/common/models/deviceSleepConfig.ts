export interface DeviceSleepConfig {
  daily: SleepConfig;
  weekend: SleepConfig;
}

export interface SleepConfig {
  startTime: string;
  duration: number;
}
