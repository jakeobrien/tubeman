import React, { Component } from 'react';
import logo from './logo.svg';
import './App.css';
import { UserStatus, AuthToggle, SetNameForm, WaitingForFightView, MidFightView, FightOverView } from './Components.js';
import * as firebase from 'firebase';


var AppState = {
    WaitingForFight: 0,
    MidFight: 1,
    FightEnded: 2
}

class App extends Component {

    constructor() {
        super();
        this.data = {
            appState: 0,
            winnerIs1: true,
            tubeman1: {
                name: "blue",
                odds: "2:1"
            },
            tubeman2: {
                name: "green",
                odds: "1:2"
            },
            user: {
                name: null,
                bank: 100,
                isLoggedIn: true,
                bet1: {
                    amount: 10,
                    payout: 20
                },
                bet2: {
                    amount: 0,
                    payout: 0
                }
            }
        };
    }

    render() {
        var contentView = null;
        console.log(this.data.appState);
        if (this.data.user.isLoggedIn && this.data.user.name === null) {
            contentView = ( <SetNameForm /> );
        } else if (this.data.appState === AppState.WaitingForFight) {
            contentView = ( <WaitingForFightView /> );
        } else if (this.data.appState === AppState.MidFight) {
            contentView = ( <MidFightView 
                                tubeman1={this.data.tubeman1} 
                                tubeman2={this.data.tubeman2} 
                                user={this.data.user} /> );
        } else if (this.data.appState === AppState.FightEnded) {
            var winner = tubeman2;
            if (this.data.winnerIs1) winner = tubeman1;
            contentView = ( <FightOverView 
                                tubeman1={this.data.tubeman1} 
                                tubeman2={this.data.tubeman2} 
                                winner={winner} 
                                user={this.data.user} /> );
        } 
        return (
            <div className="App">
                <UserStatus user={this.data.user} />
                {contentView}
                <AuthToggle />
            </div>
        );
    }
}

export default App;
