import { Component, OnInit } from '@angular/core';

import { Examples } from './shared/examples.model';
import { ExamplesService } from './shared/examples.service';

@Component({
	selector: 'examples',
	templateUrl: 'examples.component.html',
	providers: [ExamplesService]
})

export class ExamplesComponent implements OnInit {
	examples: Examples[] = [];

	constructor(private examplesService: ExamplesService) { }

	ngOnInit() {

	}
}