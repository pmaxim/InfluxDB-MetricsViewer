/**
 * This file contains authentication parameters. Contents of this file
 * is roughly the same across other MSAL.js libraries. These parameters
 * are used to initialize Angular and MSAL Angular configurations in
 * in app.module.ts file.
 */

import { LogLevel, Configuration, BrowserCacheLocation } from '@azure/msal-browser';

const isIE = window.navigator.userAgent.indexOf('MSIE ') > -1 || window.navigator.userAgent.indexOf('Trident/') > -1;

/**
 * Configuration object to be passed to MSAL instance on creation.
 * For a full list of MSAL.js configuration parameters, visit:
 * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/configuration.md
 */
/*export const msalConfig: Configuration = {
  auth: {
    clientId: 'fd836b1b-f7ef-4237-be95-82bc9f6b8ca8', // This is the ONLY mandatory field that you need to supply.
    authority: 'https://login.microsoftonline.com/f9da612b-afa8-483a-b8ae-2e453d960ffb', // Defaults to "https://login.microsoftonline.com/common"
    redirectUri: document.baseURI, // Points to window.location.origin. You must register this URI on Azure portal/App Registration.
  },
  cache: {
    cacheLocation: BrowserCacheLocation.LocalStorage, // Configures cache location. "sessionStorage" is more secure, but "localStorage" gives you SSO between tabs.
    storeAuthStateInCookie: isIE, // Set this to "true" if you are having issues on IE11 or Edge
  },
  system: {
    loggerOptions: {
      loggerCallback(logLevel: LogLevel, message: string) {
        //console.log(message);
      },
      logLevel: LogLevel.Verbose,
      piiLoggingEnabled: false
    }
  }
}*

/**
 * Add here the endpoints and scopes when obtaining an access token for protected web APIs. For more information, see:
 * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/resources-and-scopes.md
 */
/*export const protectedResources = {
  todoListApi: {
    endpoint: 'MetaMetrics',
    scopes: ['api://07d6bd0b-a286-4fee-8245-3695c634927d/access_as_user'],
  },
}*/

export const msalConfig: Configuration = {
  auth: {
    clientId: 'e9ee61d8-0964-42fc-944f-f791d5fb195f', // This is the ONLY mandatory field that you need to supply.
    authority: 'https://login.microsoftonline.com/b223f078-6fa8-4442-8e6f-825e420d4bc9', // Defaults to "https://login.microsoftonline.com/common"
    redirectUri: '/', // Points to window.location.origin. You must register this URI on Azure portal/App Registration.
  },
  cache: {
    cacheLocation: BrowserCacheLocation.LocalStorage, // Configures cache location. "sessionStorage" is more secure, but "localStorage" gives you SSO between tabs.
    storeAuthStateInCookie: isIE, // Set this to "true" if you are having issues on IE11 or Edge
  },
  system: {
    loggerOptions: {
      loggerCallback(logLevel: LogLevel, message: string) {
        //console.log(message);
      },
      logLevel: LogLevel.Verbose,
      piiLoggingEnabled: false
    }
  }
}

export const protectedResources = {
  todoListApi: {
    endpoint: 'MetaMetrics',
    scopes: ['api://345bc9e5-1013-4fd2-91da-d1bc34d99629/access_as_metametricsviewer'],
  },
}

/**
 * Scopes you add here will be prompted for user consent during sign-in.
 * By default, MSAL.js will add OIDC scopes (openid, profile, email) to any login request.
 * For more information about OIDC scopes, visit:
 * https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-permissions-and-consent#openid-connect-scopes
 */
export const loginRequest = {
  scopes: []
};
