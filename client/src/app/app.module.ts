import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavbarComponent } from './components/navbar/navbar.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HomeComponent } from './components/home/home.component';
import { RegisterComponent } from './components/register/register.component';
import { ListsComponent } from './components/lists/lists.component';
import { MemberDetailComponent } from './components/member-detail/member-detail.component';
import { MembersComponent } from './components/members/members.component';
import { MessagesComponent } from './components/messages/messages.component';
import { SharedModule } from './modules/shared.module';

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    HomeComponent,
    RegisterComponent,
    ListsComponent,
    MemberDetailComponent,
    MembersComponent,
    MessagesComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
