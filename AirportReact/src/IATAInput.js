import React, { useReducer } from 'react';
import { TextField, IconButton, CircularProgress} from '@material-ui/core';
import SendIcon from '@material-ui/icons/Send';
import { Map } from 'immutable';
import { useApi, Api} from './Api';


export function IATAInput(props) {

    function nextValue(value) {
        value = value.toUpperCase();
        value = value.replace(/[^A-Z]+/g,"");
        return value.substring(0,3);
    }

    function onChange(evt) {
        const val = nextValue(evt.target.value)
        if(props.onChange) props.onChange(val)
    }

    return <TextField
        disabled={props.disabled}
        label={props.label}
        value={props.value}
        onChange={onChange}
        className='iata-input'
        style={{marginLeft: "2px", marginRight: "2px"}}
        margin="normal"
        variant="outlined"
    />;
}

export function IATAPair() {
    const api = useApi()

    const initialState = Map({
        'iata1': '',
        'iata2': ''
    })
    function reducer(state, action) {
        if(action.iata === 1 || action.iata === 2) {
            return state.set(`iata${action.iata}`, action.value)
        }
        return state
    }

    const [state, dispatch] = useReducer(reducer, initialState)

    function onSubmit() {
        Api.getDistance(state.get('iata1'), state.get('iata2'))
    }

    return <div>
        <IATAInput label="IATA code 1" 
            value={state.get('iata1')}
            disabled={api.loading}
            onChange={(x) => dispatch({iata: 1, value: x})}
        />
        <IATAInput label="IATA code 2"
            disabled={api.loading}
            value={state.get('iata2')}
            onChange={(x) => dispatch({iata: 2, value: x})}
        />
        <IconButton color="primary" style={{display: 'contents', color: 'rgb(38, 167, 230)'}}
            disabled={api.loading}
            onClick={onSubmit}>
                {!api.loading? <SendIcon /> : <CircularProgress /> }
        </IconButton>
    </div>
}