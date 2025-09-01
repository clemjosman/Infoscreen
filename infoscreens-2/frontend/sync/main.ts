// Services
import { CacheService } from "./services/cacheService";
import { LoggingService } from "./services/loggingService";
import { NodeService } from "./services/nodeService";

const appInsights = require("applicationinsights"); // Must be the first line

// Setting up appInsights
appInsights.setup("cca99f16-cc9c-4114-bd56-f9c4a9d4dbd9");
appInsights.start();
appInsights.defaultClient.context.tags["ai.cloud.role"] = "vesact-infoscreens";
appInsights.defaultClient.context.tags["ai.cloud.roleInstance"] =
  NodeService.getNodeId();

// Start main sync process
let retryCount = 0;
const main = () => {
  try {
    let cacheService = new CacheService();
    cacheService.initAndStartCachesUpdateAsync();
  } catch (error) {
    LoggingService.fatal(
      "Sync",
      "Main",
      "An error occured in the main file",
      error
    );
    if (retryCount++ < 25) {
      main();
    }
  }
};
main();
