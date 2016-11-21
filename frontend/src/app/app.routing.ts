import { NgModule }             from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AuthComponent }   from './auth/auth.component';
import { CompareComponent }      from './compare/compare.component';
import { ConfiguratorComponent }  from './configurator/configurator.component';
import { ExamplesComponent }      from './examples/examples.component';

const routes: Routes = [
  { path: '', redirectTo: '/configurator', pathMatch: 'full' },
  { path: 'configurator',  component: ConfiguratorComponent },
  { path: 'compare', component: CompareComponent },
  { path: 'auth',     component: AuthComponent },
  { path: 'examples',     component: ExamplesComponent }
];

@NgModule({
  imports: [ RouterModule.forRoot(routes) ],
  exports: [ RouterModule ]
})

export class AppRouting {}
