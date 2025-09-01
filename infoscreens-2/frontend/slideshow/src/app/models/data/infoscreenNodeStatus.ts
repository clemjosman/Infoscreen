export interface InfoscreenNodeStatus {
  id: string;
  name: string;
  enabled: boolean;
  protocol: string;
  npmVersion: string;
  debug: string;
  isOnlineMsb: string;
  deviceId: string;
  connectionState: string;
  status: string;
  uiVersion: string;
  desiredUiVersion: string;
  tags: string[];
}
