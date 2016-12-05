import { Component } from '@angular/core';
import { UploadService } from './shared/upload.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less'],
  providers: [ UploadService ]
})
export class AppComponent {
  title = 'PC Config!';
  navList: INavItem[] = [];
  auth: INavItem;
  myColor = "primary";
  //"primary" | "accent" | "warn"
  isAuth:boolean;
  picName: any;

  constructor(private service: UploadService) {
    this.navList = [
      { address: "/configurator", text: "Сборка" },
      { address: "/examples", text: "Шаблоны" },
      { address: "/compare", text: "Сравнение" },
    ]
    this.auth = { address: "/auth", text: "Авторизация" };
    this.isAuth = false;

    this.service.progress$.subscribe(
      (data) => {
        console.log('progress = ' + data);
      }
    );
  }
  


  onChange(event) {
    console.log('onChange', event);
    var files = event.srcElement.files;
    console.log(files);
    this.service.makeFileRequest('http://localhost:27835/api/upload', [], files).subscribe(
      (fileName) => {
        console.log('sent');
        this.picName = fileName;
      }
    );
  }  

}

interface INavItem {
  text: string;
  address: string;
}
