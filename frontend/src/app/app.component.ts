import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})
export class AppComponent {
  title = 'PC Config!';
  navList: INavItem[] = [];
  myColor = "primary"; 
  //"primary" | "accent" | "warn"

  constructor() {
    this.navList = [
      { address: "/configurator", text: "Сборка" },
      { address: "/compare", text: "Сравнение" },
      { address: "/examples", text: "Шаблоны" },

      { address: "/auth", text: "Авторизация" }
    ]
  }
}

interface INavItem {
  text: string;
  address: string;
}
