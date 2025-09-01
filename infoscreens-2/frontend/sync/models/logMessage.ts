import { LogLevelStrings, LogOrigin } from '../../common';

export class LogMessage {
    origin: LogOrigin;
    logLevel: LogLevelStrings;
    quickContext: string;
    context: string;
    message: string;
    exceptionType: string;
    exceptionMessage: string;
    exceptionStack: string;
    custom1: string;
    custom2: string;
    custom3: string;
    custom4: string;
}
