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
	price: number;
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
	isLoading: boolean;

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
		this.price = 0;
		this.isLoading = false;
	 }

	ngOnInit() {
		
	}

	click(item) {
		this.isLoading = true;
		this.items = [];
		this.configuratorService.getNames(item.type)
			.subscribe(
				(res) => {
					this.curType = item.type;
					this.setList(item.type, res);
				},
				(err) => {
					console.log(err);	
				}
			);
	}

	setList(type, names) {
		let pcDATA = {
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

		pcDATA.CPU = this.pc.CPU ? 						parseInt(this.pc.CPU.id) 			: -1;
		pcDATA.motherboard = this.pc.motherboard ? 		parseInt(this.pc.motherboard.id) 	: -1;
		pcDATA.RAM = this.pc.RAM ? 						parseInt(this.pc.RAM.id) 			: -1;
		pcDATA.videocard = this.pc.videocard ? 			parseInt(this.pc.videocard.id)		: -1;
		pcDATA.Power = this.pc.Power ? 					parseInt(this.pc.Power.id) 			: -1;
		pcDATA.HDD = this.pc.HDD ? 						parseInt(this.pc.HDD.id)			: -1;
		pcDATA.body = this.pc.body ? 					parseInt(this.pc.body.id) 			: -1;
		pcDATA.coolingSystem = this.pc.coolingSystem ? 	parseInt(this.pc.coolingSystem.id) 	: -1;
		pcDATA.accessories = this.pc.accessories ? 		parseInt(this.pc.accessories.id) 	: -1;

		
		this.configuratorService.getList(type, pcDATA)
			.subscribe(
				(res: any[]) => {
					res.forEach((val) => {
						let tmp = { 
							id: val[0],
							model: "Модель: " + val[1], 
							type: type, 
							brend: "Бренд " + val[2],
							text: "",
							price: 0
						};
						names.forEach((name, key) => {
							if(name != null && name != "Price") {
								tmp.text += name + ": " + val[key + 3] + "; ";
							}
							if(name == "Price") {
								tmp.price = parseInt(val[key + 3]);
							}
						})
						this.items.push(tmp);
					});
					this.isLoading = false;
				},
				(err) => {
					console.log(err);	
				}
			);
	}

	add(item) {
		this.pc[this.curType] = item;
		this.items = [];
		this.curType = null;
		this.culcPrice();
	}

	del(name) {
		this.pc[name] = null;
		this.culcPrice();
	}

	culcPrice() {
		this.price = 0;
		for (var key in this.pc) {
			if(this.pc[key] != null)
				this.price += this.pc[key].price;
		}
	}
}