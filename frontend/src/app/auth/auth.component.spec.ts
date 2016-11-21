import { TestBed, inject } from '@angular/core/testing';
import { HttpModule } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';

import { AuthComponent } from './auth.component';
import { AuthService } from './shared/auth.service';
import { Auth } from './shared/auth.model';

describe('a auth component', () => {
	let component: AuthComponent;

	// register all needed dependencies
	beforeEach(() => {
		TestBed.configureTestingModule({
			imports: [HttpModule],
			providers: [
				{ provide: AuthService, useClass: MockAuthService },
				AuthComponent
			]
		});
	});

	// instantiation through framework injection
	beforeEach(inject([AuthComponent], (AuthComponent) => {
		component = AuthComponent;
	}));

	it('should have an instance', () => {
		expect(component).toBeDefined();
	});
});

// Mock of the original auth service
class MockAuthService extends AuthService {
	getList(): Observable<any> {
		return Observable.from([ { id: 1, name: 'One'}, { id: 2, name: 'Two'} ]);
	}
}
