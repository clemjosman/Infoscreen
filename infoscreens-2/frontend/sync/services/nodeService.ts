import { OsService } from "./osService";
var jetpack = require("fs-jetpack");

export class NodeService {
  static getNodeId() {
    var filePath = OsService.getNodeFilePath();
    if (jetpack.exists(filePath)) {
      return JSON.parse(jetpack.read(filePath)).msbNodeName;
    } else {
      throw "Could not read nodeId. Config file not found at " + filePath;
    }
  }

  static getNodeMaintenanceMode() {
    var filePath = OsService.getNodeFilePath();
    if (jetpack.exists(filePath)) {
      return JSON.parse(jetpack.read(filePath)).maintenanceMode;
    } else {
      throw (
        "Could not read maintenance mode. Config file not found at " + filePath
      );
    }
  }

  static getNodeMaintenanceEndDate() {
    var filePath = OsService.getNodeFilePath();
    if (jetpack.exists(filePath)) {
      return JSON.parse(jetpack.read(filePath)).maintenanceEndDate;
    } else {
      throw (
        "Could not read maintenance end date. Config file not found at " +
        filePath
      );
    }
  }

  // the returned url will not end with a slash (/)
  static getAzureFunctionEndpoint() {
    var filePath = OsService.getNodeFilePath();
    if (jetpack.exists(filePath)) {
      return JSON.parse(jetpack.read(filePath)).azureFunctionEndpoint;
    } else {
      throw (
        "Could not read Azure Function endpoint url. Config file not found at " +
        filePath
      );
    }
  }

  static getNodeTags() {
    var filePath = OsService.getNodeFilePath();
    if (jetpack.exists(filePath)) {
      return JSON.parse(jetpack.read(filePath)).nodeTags;
    } else {
      throw "Could not read nodeTags. Config file not found at " + filePath;
    }
  }
}
