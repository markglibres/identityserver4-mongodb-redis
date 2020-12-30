import qs from 'qs';

const queryParams = qs.parse(window.location.search, { ignoreQueryPrefix: true });

const getQueryString = (query: string): string => queryParams[query] as string;

export { getQueryString };
