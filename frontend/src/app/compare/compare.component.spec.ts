import { TestBed, inject } from '@angular/core/testing';
import { HttpModule } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';

import { CompareComponent } from './compare.component';
import { CompareService } from './shared/compare.service';
import { Compare } from './shared/compare.model';

describe('a compare component', () => {
	let component: CompareComponent;

	// register all needed dependencies
	beforeEach(() => {
		TestBed.configureTestingModule({
			imports: [HttpModule],
			providers: [
				{ provide: CompareService, useClass: MockCompareService },
				CompareComponent
			]
		});
	});

	// instantiation through framework injection
	beforeEach(inject([CompareComponent], (CompareComponent) => {
		component = CompareComponent;
	}));

	it('should have an instance', () => {
		expect(component).toBeDefined();
	});
});

// Mock of the original compare service
class MockCompareService extends CompareService {
	getList(): Observable<any> {
		return Observable.from([ { id: 1, name: 'One'}, { id: 2, name: 'Two'} ]);
	}
}
