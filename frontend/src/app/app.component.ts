import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})
export class AppComponent {
  title = 'PC Config!';
  navList: INavItem[] = [];
  auth: INavItem;
  myColor = "primary";
  isAuth:boolean;
  //"primary" | "accent" | "warn"

  constructor() {
    this.navList = [
      { address: "/configurator", text: "Сборка" },
      { address: "/examples", text: "Шаблоны" },
      { address: "/compare", text: "Сравнение" },
    ]
    this.auth = { address: "/auth", text: "Авторизация" };
    this.isAuth = false;
    
  }
}

interface INavItem {
  text: string;
  address: string;
}
