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
        this.state = {
            appState: 2,
            winner: 0,
            tubeman1: null,
            tubeman2: null,
            user: {
                name: "t",
                bank: 0,
                isLoggedIn: true,
                bet1: {
                    amount: 0,
                    payout: 0
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

        if (this.state.user.isLoggedIn && this.state.user.name === null) {
            contentView = ( <SetNameForm /> );

        } else if (this.state.appState === AppState.WaitingForFight) {
            contentView = ( <WaitingForFightView /> );

        } else if (this.state.appState === AppState.MidFight) {
            contentView = ( <MidFightView 
                                tubeman1={this.state.tubeman1} 
                                tubeman2={this.state.tubeman2} 
                                user={this.state.user} /> );

        } else if (this.state.appState === AppState.FightEnded) {
            var winner = null;
            if (this.state.winner === 1) winner = this.state.tubeman1;
            else if (this.state.winner === 2) winner = this.state.tubeman2;
            contentView = ( <FightOverView 
                                tubeman1={this.state.tubeman1} 
                                tubeman2={this.state.tubeman2} 
                                winner={winner} 
                                user={this.state.user} /> );
        } 

        return (
            <div className="App">
                <UserStatus user={this.state.user} />
                {contentView}
                <AuthToggle isLoggedIn={this.state.user.isLoggedIn} />
            </div>
        );
    }
}

export default App;
