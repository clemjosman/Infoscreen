export class InfoscreenCachedStatus {
  // Node info (mSB)
  id: string;
  name: string;
  connectionId: string;
  machineName: string;
  details: string;
  publicIp: string;
  enabled: boolean;
  organizationId: string;
  lockToMachine: boolean;
  protocol: string;
  npmVersion: string;
  debug: boolean;
  webPort: number;
  mode: string;
  tags: string[];
  platform: string;
  longitude: string;
  latitude: string;
  iccid: string;
  manufatureId: string;
  imei: string;
  allowSend: boolean;
  timeZone: string;
  retentionPeriod: number;

  // Connection state (mSB)
  isOnlineMsb: boolean;
  deviceId: string;
  generationId: string;
  etag: string;
  connectionState: string;
  status: string;
  statusReason: string;
  connectionStateUpdatedTime: string;
  statusUpdatedTime: string;
  lastActivityTime: string;
  cloudToDeviceMessageCount: number;

  // Device state (vesact)
  uiVersion: string;
  desiredUiVersion: string;
  firmwareVersion: string;
  desiredFirmwareVersion: string;
}
