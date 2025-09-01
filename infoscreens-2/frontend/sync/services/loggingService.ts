import { LogMessage } from '../models/logMessage';
import { ConfigHelper } from '../helpers/configHelper';
import { LogLevel, LogOrigin } from '../../common';

export class LoggingService {
    private static logMessage(logMessage: LogMessage): void {
        // Check log level with configured one
        if (ConfigHelper.config) {
            if (logMessage.origin == LogOrigin.Sync) {
                if (Number(LogLevel[ConfigHelper.config.syncLogLevel]) > Number(LogLevel[logMessage.logLevel])) {
                    return;
                }
            }
        }
        var logMessageString = JSON.stringify(logMessage);
        this.simpleStdoutLog(logMessageString);
    }

    private static log(
        logLevel: LogLevel,
        quickContext: string,
        context: string,
        message: string,
        logOrigin: LogOrigin,
        custom1: string = undefined,
        custom2: string = undefined,
        custom3: string = undefined,
        custom4: string = undefined,
        error: Error = undefined
    ): void {
        try {
            var logMessage: LogMessage = {
                origin: logOrigin,
                logLevel: LogLevel[logLevel],
                quickContext: quickContext,
                context: context,
                message: message,
                custom1: custom1,
                custom2: custom2,
                custom3: custom3,
                custom4: custom4
            } as LogMessage;

            if (error) {
                logMessage.exceptionType = error.name;
                logMessage.exceptionMessage = error.message;
                logMessage.exceptionStack = error.stack;
            }

            this.logMessage(logMessage);
        } catch (ex) {
            this.simpleStdoutLog(
                `An error occured when trying to log ${
                    error ? 'error' : 'message'
                } with loglevel: ${logLevel}, quickContext: ${quickContext}, context: ${context}, message: ${message}, logOrigin: ${logOrigin} ${
                    error ? 'error: ' + JSON.stringify(error) : ''
                }`,
                ex
            );
        }
    }

    static trace(
        quickContext: string,
        context: string,
        message: string,
        custom1: string = undefined,
        custom2: string = undefined,
        custom3: string = undefined,
        custom4: string = undefined,
        origin: LogOrigin = LogOrigin.Sync
    ) {
        this.log(LogLevel.trace, quickContext, context, message, origin, custom1, custom2, custom3, custom4);
    }

    static debug(
        quickContext: string,
        context: string,
        message: string,
        custom1: string = undefined,
        custom2: string = undefined,
        custom3: string = undefined,
        custom4: string = undefined,
        origin: LogOrigin = LogOrigin.Sync
    ) {
        this.log(LogLevel.debug, quickContext, context, message, origin, custom1, custom2, custom3, custom4);
    }

    static info(
        quickContext: string,
        context: string,
        message: string,
        custom1: string = undefined,
        custom2: string = undefined,
        custom3: string = undefined,
        custom4: string = undefined,
        origin: LogOrigin = LogOrigin.Sync
    ) {
        this.log(LogLevel.info, quickContext, context, message, origin, custom1, custom2, custom3, custom4);
    }

    static warn(
        quickContext: string,
        context: string,
        message: string,
        custom1: string = undefined,
        custom2: string = undefined,
        custom3: string = undefined,
        custom4: string = undefined,
        origin: LogOrigin = LogOrigin.Sync
    ) {
        this.log(LogLevel.warn, quickContext, context, message, origin, custom1, custom2, custom3, custom4);
    }

    static error(
        quickContext: string,
        context: string,
        message: string,
        error: Error,
        custom1: string = undefined,
        custom2: string = undefined,
        custom3: string = undefined,
        custom4: string = undefined,
        origin: LogOrigin = LogOrigin.Sync
    ) {
        this.log(LogLevel.error, quickContext, context, message, origin, custom1, custom2, custom3, custom4, error);
    }

    static fatal(
        quickContext: string,
        context: string,
        message: string,
        error: Error,
        custom1: string = undefined,
        custom2: string = undefined,
        custom3: string = undefined,
        custom4: string = undefined,
        origin: LogOrigin = LogOrigin.Sync
    ) {
        this.log(LogLevel.fatal, quickContext, context, message, origin, custom1, custom2, custom3, custom4, error);
    }

    /**
     * Logs message
     * @param message
     * @param error
     */
    private static simpleStdoutLog(message: string, error: Error = null) {
        console.log(message);
        if (error) {
            console.log(error.message);
            console.log(error.name);
            console.log(error.stack);
        }

        // Using process.stdout.write to transmit log message to the mSB script
        // Not needed for docker
        /*process.stdout.write(message);
    if (error) {
      process.stdout.write(error.message);
      process.stdout.write(error.name);
      process.stdout.write(error.stack);
    }*/
    }
}
