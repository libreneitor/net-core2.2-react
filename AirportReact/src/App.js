import React from 'react';
import {Container} from '@material-ui/core';
import Brand from './Brand';
import { IATAPair } from './IATAInput'
import './App.css'
import { DistanceList } from './DistanceDisplay';

function App() {
  return (
    <Container maxWidth="sm" style={{textAlign: 'center'}}>
      <Brand />
      <IATAPair/>
      <DistanceList />
    </Container>
  );
}


export default App;
