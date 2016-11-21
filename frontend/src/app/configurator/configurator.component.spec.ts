import { TestBed, inject } from '@angular/core/testing';
import { HttpModule } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';

import { ConfiguratorComponent } from './configurator.component';
import { ConfiguratorService } from './shared/configurator.service';
import { Configurator } from './shared/configurator.model';

describe('a configurator component', () => {
	let component: ConfiguratorComponent;

	// register all needed dependencies
	beforeEach(() => {
		TestBed.configureTestingModule({
			imports: [HttpModule],
			providers: [
				{ provide: ConfiguratorService, useClass: MockConfiguratorService },
				ConfiguratorComponent
			]
		});
	});

	// instantiation through framework injection
	beforeEach(inject([ConfiguratorComponent], (ConfiguratorComponent) => {
		component = ConfiguratorComponent;
	}));

	it('should have an instance', () => {
		expect(component).toBeDefined();
	});
});

// Mock of the original configurator service
class MockConfiguratorService extends ConfiguratorService {
	getList(): Observable<any> {
		return Observable.from([ { id: 1, name: 'One'}, { id: 2, name: 'Two'} ]);
	}
}
