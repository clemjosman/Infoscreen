import { Injectable } from '@angular/core';
import { ConfigService } from './config.service';
import { LogMessage } from '../models/logMessage';
import { LogLevel } from '../../../../common';

@Injectable({
    providedIn: 'root'
})
export class LoggingService {
    // No check of the configured log level is done here, this is the task of the mSB script
    private static log(
        logLevel: LogLevel,
        quickContext: string,
        context: string,
        message: string,
        custom1: string = undefined,
        custom2: string = undefined,
        custom3: string = undefined,
        custom4: string = undefined,
        error: Error = undefined
    ) {
        // Check log level with configured one
        if (ConfigService.config) {
            if (Number(LogLevel[ConfigService.config.uiLogLevel]) > logLevel) {
                return;
            }
        }

        var logMessage = {
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

        // Using different log levels to transmit log message to the parent process via chromium
        switch (LogLevel[logMessage.logLevel]) {
            case LogLevel.trace:
                console.trace(JSON.stringify(logMessage));
                break;
            case LogLevel.debug:
                console.debug(JSON.stringify(logMessage));
                break;
            case LogLevel.info:
                console.info(JSON.stringify(logMessage));
                break;
            case LogLevel.warn:
                console.warn(JSON.stringify(logMessage));
                break;
            case LogLevel.error:
                console.error(JSON.stringify(logMessage));
                break;
            case LogLevel.fatal:
                console.error(JSON.stringify(logMessage));
                break;
            default:
                break;
        }
    }

    static trace(
        quickContext: string,
        context: string,
        message: string,
        custom1: string = undefined,
        custom2: string = undefined,
        custom3: string = undefined,
        custom4: string = undefined
    ) {
        this.log(LogLevel.trace, quickContext, context, message, custom1, custom2, custom3, custom4);
    }

    static debug(
        quickContext: string,
        context: string,
        message: string,
        custom1: string = undefined,
        custom2: string = undefined,
        custom3: string = undefined,
        custom4: string = undefined
    ) {
        this.log(LogLevel.debug, quickContext, context, message, custom1, custom2, custom3, custom4);
    }

    static info(
        quickContext: string,
        context: string,
        message: string,
        custom1: string = undefined,
        custom2: string = undefined,
        custom3: string = undefined,
        custom4: string = undefined
    ) {
        this.log(LogLevel.info, quickContext, context, message, custom1, custom2, custom3, custom4);
    }

    static warn(
        quickContext: string,
        context: string,
        message: string,
        custom1: string = undefined,
        custom2: string = undefined,
        custom3: string = undefined,
        custom4: string = undefined
    ) {
        this.log(LogLevel.warn, quickContext, context, message, custom1, custom2, custom3, custom4);
    }

    static error(
        quickContext: string,
        context: string,
        message: string,
        error: Error,
        custom1: string = undefined,
        custom2: string = undefined,
        custom3: string = undefined,
        custom4: string = undefined
    ) {
        this.log(LogLevel.error, quickContext, context, message, custom1, custom2, custom3, custom4, error);
    }

    static fatal(
        quickContext: string,
        context: string,
        message: string,
        error: Error,
        custom1: string = undefined,
        custom2: string = undefined,
        custom3: string = undefined,
        custom4: string = undefined
    ) {
        this.log(LogLevel.fatal, quickContext, context, message, custom1, custom2, custom3, custom4, error);
    }
}
