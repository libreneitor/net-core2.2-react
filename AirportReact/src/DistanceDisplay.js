import React from 'react';
import { ListItem, ListItemText, List } from '@material-ui/core';
import { useApi } from './Api';

function DistanceItem(props) {
    const r = props.result
    const primary = r.distance?
        `${r.distance} Miles` :
        "Not found";
    const secondary = `${r.iata1}-${r.iata2}`

    return (
        <ListItem>
            <ListItemText primary={primary} secondary={secondary} />
        </ListItem>
    );
}

export function DistanceList() {
    const api = useApi()
    return (
        <List>
            {api.results.map((result, idx) => 
                <DistanceItem result={result} key={idx}/> 
            )}
        </List>
    )
}