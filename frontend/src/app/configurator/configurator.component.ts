import { Component, OnInit } from '@angular/core';

import { Configurator } from './shared/configurator.model';
import { ConfiguratorService } from './shared/configurator.service';

@Component({
	selector: 'configurator',
	templateUrl: 'configurator.component.html',
	providers: [ConfiguratorService]
})

export class ConfiguratorComponent implements OnInit {
	configurator: Configurator[] = [];
	devices: any[];
	items: any[];
	pc = {
		proc: null,
		mat: null,
		oper: [],
		video: null,
		block: null,
		hdd: [],
		korp: null,
		oh: [],
		acc: [],
	}

	constructor(private configuratorService: ConfiguratorService) {
		this.devices = [
			"Процессор *",
			"Материнская плата *",
			"Оперативная память *",
			"Видео-карта *",
			"Блок питания *",
			"Жёсткий диск",
			"Корпус",
			"Охлаждение",
			"Аксессуары"
		]
		this.items = [];
		for(let i = 0; i < 100; i++) {
			let tmp = { model: "Модель " + i, type: "Тип " + i, brend: "Бренд " + i };
			this.items.push(tmp);
		}
	 }

	ngOnInit() {
	}
}