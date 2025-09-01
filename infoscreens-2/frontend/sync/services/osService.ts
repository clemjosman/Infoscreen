const DOCKER_ENV_VARIABLE = "NODEJS_DOCKER_ENV";
const DOCKER_ENV_VALUE = "true";

const NODE_FILE_PATH_DOCKER = "/usr/files/config/node.json";
const NODE_FILE_PATH_LINUX = "/data/infoscreen/config/node.json";
const NODE_FILE_PATH_WIN =
  __dirname.split("\\").slice(0, -6).join("\\") + "\\config\\node.json";

const CACHED_FOLDER_PATHS_DOCKER = ["/usr/src/app/cache/"];
const CACHED_FOLDER_PATHS_LINUX = [
  __dirname.split("/").slice(0, -6).join("/") + "/dist/cache/",
];
const CACHED_FOLDER_PATHS_WIN = [
  __dirname.split("\\").slice(0, -6).join("\\") + "\\dist\\cache\\",
  __dirname.split("\\").slice(0, -6).join("\\") + "\\src\\cache\\",
];

export class OsService {
  constructor() {}

  static isOsLinux(): boolean {
    return process.platform == "linux";
  }

  static isEnvDocker(): boolean {
    return process.env[DOCKER_ENV_VARIABLE] === DOCKER_ENV_VALUE;
  }

  static getNodeFilePath(): string {
    if (OsService.isEnvDocker()) return NODE_FILE_PATH_DOCKER;
    if (OsService.isOsLinux()) return NODE_FILE_PATH_LINUX;
    return NODE_FILE_PATH_WIN;
  }

  static getCachedFolderPathArray() {
    if (OsService.isEnvDocker()) return CACHED_FOLDER_PATHS_DOCKER;
    if (OsService.isOsLinux()) return CACHED_FOLDER_PATHS_LINUX;
    return CACHED_FOLDER_PATHS_WIN;
  }
}
