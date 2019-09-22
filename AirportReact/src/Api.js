import { BehaviorSubject } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { fromFetch } from 'rxjs/fetch';
import { Map } from 'immutable';
import { useEffect, useState } from 'react';

class _Api {
    constructor() {
        this.state = Map({
            loading: false,
            results: []
        })
        this.subject = new BehaviorSubject(this.state.toObject())
    }

    getSubject() {
        return this.subject;
    }

    getDistance(iata1, iata2) {
        if(this.state.get('loading')){
            return;
        }
        this._setState('loading', true)
        return fromFetch(`${iata1}/${iata2}`)
            .pipe(
                switchMap(x => {
                    this._setState('loading', false)
                    return x.json()
                })
            )
            .subscribe(r => {
                const result = {iata1, iata2, distance: r.distance}
                const results = [result].concat(this.state.get('results'))
                this._setState('results', results)
            })
    }

    _setState(name, value) {
        this.state = this.state.set(name, value)
        this.subject.next(this.state.toObject())
    }

}


export const Api = new _Api()
export function useApi() {
    const [state, setState] = useState(Api.state.toObject())
    useEffect(() => {
        const sub = Api.getSubject().subscribe(val => {
            setState(val)
        })
        return () => sub.unsubscribe()
    })
    return state
}