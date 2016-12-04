import { Component, OnInit } from '@angular/core';

import { Configurator } from './shared/configurator.model';
import { ConfiguratorService } from './shared/configurator.service';

@Component({
	selector: 'configurator',
	templateUrl: 'configurator.component.html',
	styleUrls: ['./configurator.component.less'],
	providers: [ConfiguratorService]
})

export class ConfiguratorComponent implements OnInit {
	configurator: Configurator[] = [];
	devices: any[];
	items: any[];
	pc = {
		CPU: null,
		motherboard: null,
		RAM: null,
		videocard: null,
		Power: null,
		HDD: null,
		body: null,
		coolingSystem: null,
		accessories: null
	};
	curType: string;

	constructor(private configuratorService: ConfiguratorService) {
		this.devices = [
			{ text: "Процессор *", 				type: "CPU", 			address: "1"},
			{ text: "Материнская плата *", 		type: "motherboard", 	address: "1"},
			{ text: "Оперативная память *", 	type: "RAM", 			address: "1"},
			{ text: "Видеокарта *", 			type: "videocard", 		address: "1"},
			{ text: "Блок питания *", 			type: "Power", 			address: "1"},
			{ text: "Жёсткий диск", 			type: "HDD", 			address: "1"},
			{ text: "Корпус", 					type: "body", 			address: "1"},
			{ text: "Охлаждение", 				type: "coolingSystem", 	address: "1"},
			{ text: "Аксессуары", 				type: "accessories", 	address: "1"}

		]
		this.items = [];
	 }

	ngOnInit() {
		this.configuratorService.getList()
		.subscribe(
			(res) => {
				console.log(res);
			},
			(err) => {
				console.log(err);	
			}
		);
	}

	click(item) {
		this.curType = item.type;
		this.items = [];
		for(let i = 0; i < 50; i++) {
			let tmp = { model: "Модель " + item.text + i, type: item.type, brend: "Бренд " + i };
			this.items.push(tmp);
		}
	}

	add(item) {
		this.pc[this.curType] = item;
		this.items = [];
		this.curType = null;
	}
}