import React, { Component } from 'react';

export class UserStatus extends Component {
    render() {
        if (!this.props.user || !this.props.user.isLoggedIn) return null;
        return (
            <div className="header">
                <span>{this.props.user.name}</span>
                <span>${this.props.user.bank}</span>
            </div>
        );
    }
}

export class AuthToggle extends Component {
    render() {
        var buttonText = this.props.isLoggedIn ? "Sign Out" : "Sign In";
        return (
            <div>
                <form onSubmit={this.props.onSubmit}>
                    <input type="submit" className="button" value={buttonText} />
                </form>
            </div>
        );
    }
}

export class SetNameForm extends Component {
    render() {
        return (
            <form onSubmit={this.props.onSubmit}>
                <input type="text" onChange={this.props.onNameChanged} placeholder="Enter name..." />
                <input type="submit" value="Go" />
            </form>
        );
    }
}

export class WaitingForFightView extends Component {
    render() {
        return (
            <div>
                <h2>Waiting For Fight To Start</h2>
            </div>
        );
    }
}

export class MidFightView extends Component {
    render() {
        if (!this.props.user) return null;
        return (
            <div className="content">
                <h2>Your Bets</h2>
                <div>
                    <MidFightBetView tubeman={this.props.tubeman1} bet={this.props.user.bet1} onBet={this.props.onBetTubeman1} /> 
                    <MidFightBetView tubeman={this.props.tubeman2} bet={this.props.user.bet2} onBet={this.props.onBetTubeman2} />
                </div>
            </div>
        );
    }
}

export class MidFightBetView extends Component {
    render() {
        if (!this.props.tubeman || !this.props.bet) return null;
        return (
            <div className="split-column">
                <h3>{this.props.tubeman.name}</h3>
                <div className="info-column">
                    <div>Current odds:</div>
                    <div>Current bet:</div>
                </div>
                <div className="info-column">
                    <div>{this.props.tubeman.odds}</div>
                    <div>${this.props.bet.amount}</div>
                </div>
                <BetForm onBet={this.props.onBet} />
            </div>
        );
    }
}

export class BetForm extends Component {
    render() {
        return (
            <form onSubmit={this.props.onBet}>
                <button className="button" name="4" onClick={this.props.onBet}>Bet $4</button>
                <button className="button" name="16" onClick={this.props.onBet}>Bet $16</button>
                <br/>
                <button className="button" name="64" onClick={this.props.onBet}>Bet $64</button>
                <button className="button" name="256" onClick={this.props.onBet}>Bet $256</button>
            </form>
        );
    }
}

export class FightOverView extends Component {
    render() {
        if (!this.props.winner || !this.props.user) return null;
        return (
            <div className="content">
                <h1>{this.props.winner.name} won</h1>
                <h3>at {this.props.winner.odds} odds</h3>
                <br /><br />
                <h2>Your Bets</h2>
                <div>
                        <FightOverResult tubeman={this.props.tubeman1} bet={this.props.user.bet1} />
                        <FightOverResult tubeman={this.props.tubeman2} bet={this.props.user.bet2} />
                </div>
            </div>
        );
    }
}

class FightOverResult extends Component {
    render() {
        if (!this.props.tubeman || !this.props.bet) return null;
        return (
            <div className="split-column">
                <div className="info-column">
                    <h3>{this.props.tubeman.name}:</h3>
                    <p>Odds:</p>
                    <p>Pays:</p>
                </div>
                <div className="info-column">
                    <h3>${this.props.bet.amount}</h3>
                    <p>{this.props.tubeman.odds}</p>
                    <p>${this.props.bet.payout}</p>
                </div>
            </div>
        );
    }
}
