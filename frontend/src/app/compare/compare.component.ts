import { Component, OnInit } from '@angular/core';

import { Compare } from './shared/compare.model';
import { CompareService } from './shared/compare.service';

@Component({
	selector: 'compare',
	templateUrl: 'compare.component.html',
	providers: [CompareService]
})

export class CompareComponent implements OnInit {
	compare: Compare[] = [];

	constructor(private compareService: CompareService) { }

	ngOnInit() {
	}
}