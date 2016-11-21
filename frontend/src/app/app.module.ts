import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { MaterialModule } from '@angular/material';

import { AppComponent } from './app.component';
import { AuthComponent }   from './auth/auth.component';
import { CompareComponent }      from './compare/compare.component';
import { ConfiguratorComponent }  from './configurator/configurator.component';
import { ExamplesComponent }      from './examples/examples.component';

import { AppRouting } from './app.routing';

@NgModule({
  declarations: [
    AppComponent,
    AuthComponent,
    CompareComponent,
    ConfiguratorComponent,
    ExamplesComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpModule,
    AppRouting,
    MaterialModule.forRoot()
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
