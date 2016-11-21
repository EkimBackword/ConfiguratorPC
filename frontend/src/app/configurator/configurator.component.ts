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

	constructor(private configuratorService: ConfiguratorService) { }

	ngOnInit() {
	}
}