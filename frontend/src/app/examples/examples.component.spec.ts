import { TestBed, inject } from '@angular/core/testing';
import { HttpModule } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';

import { ExamplesComponent } from './examples.component';
import { ExamplesService } from './shared/examples.service';
import { Examples } from './shared/examples.model';

describe('a examples component', () => {
	let component: ExamplesComponent;

	// register all needed dependencies
	beforeEach(() => {
		TestBed.configureTestingModule({
			imports: [HttpModule],
			providers: [
				{ provide: ExamplesService, useClass: MockExamplesService },
				ExamplesComponent
			]
		});
	});

	// instantiation through framework injection
	beforeEach(inject([ExamplesComponent], (ExamplesComponent) => {
		component = ExamplesComponent;
	}));

	it('should have an instance', () => {
		expect(component).toBeDefined();
	});
});

// Mock of the original examples service
class MockExamplesService extends ExamplesService {
	getList(): Observable<any> {
		return Observable.from([ { id: 1, name: 'One'}, { id: 2, name: 'Two'} ]);
	}
}
