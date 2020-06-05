import Vue from 'vue';
import Router, { Route } from 'vue-router';

import { Forbidden } from '@servicestack/vue';
import { Win32 } from '../components/Win32';

export enum Routes {
  Home = '/',
  Forbidden = '/forbidden',
}

Vue.use(Router);

const routes = [
  { path: Routes.Home, component: Win32 },
  { path: Routes.Forbidden, component: Forbidden },
  { path: '*', redirect: '/' },
];

export const router = new Router ({
    mode: 'history',
    linkActiveClass: 'active',
    routes,
});
export default router;

export const redirect = (path: string) => {
  const externalUrl = path.indexOf('://') >= 0;
  if (!externalUrl) {
      router.push({ path });
  } else {
      location.href = path;
  }
};
