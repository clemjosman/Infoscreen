import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from './app/app.module';
import { LoggingService } from './app/services/logging.service';

platformBrowserDynamic()
  .bootstrapModule(AppModule, {
    preserveWhitespaces: false
  })
  .catch(error => LoggingService.error("Angular Main", "platformBrowserDynamic", "An error occured within the platform browser dynamic", error));