export enum LogLevel {
  trace = 0, // Used data and functions called
  debug = 1, // Actions done on low level
  info = 2, // Actions done on high level
  warn = 3, // Handled errors
  error = 4, // Unhandled errors
  fatal = 5, // Fatal errors
  none = 6, // No log
}

export type LogLevelStrings = keyof typeof LogLevel;
