import React, { Component } from 'react';

export class UserStatus extends Component {
    render() {
        if (!this.props.user || !this.props.user.isLoggedIn) return null;
        return (
            <div>
                <span>{this.props.user.name} </span>
                <span>${this.props.user.bank}</span>
            </div>
        );
    }
}

export class AuthToggle extends Component {
    render() {
        var buttonText = this.props.isLoggedIn ? "Sign Out" : "Sign In";
        return (
            <form onSubmit={this.props.onSubmit}>
                <input type="submit" value={buttonText} />
            </form>
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
                <div>Waiting For Fight To Start</div>
            </div>
        );
    }
}

export class MidFightView extends Component {
    render() {
        if (!this.props.user) return null;
        return (
            <div>
                <div>Your bets</div>
                <table><tbody>
                    <tr>
                        <MidFightBetView tubeman={this.props.tubeman1} bet={this.props.user.bet1} /> 
                        <MidFightBetView tubeman={this.props.tubeman2} bet={this.props.user.bet2} />
                    </tr>
                </tbody></table>
            </div>
        );
    }
}

export class MidFightBetView extends Component {
    render() {
        if (!this.props.tubeman || !this.props.bet) return null;
        return (
            <td>
                <div>{this.props.tubeman.name}</div>
                <div>Current odds: {this.props.tubeman.odds}</div>
                <div>Current bet: ${this.props.bet.amount}</div>
                <BetForm />
            </td>
        );
    }
}

export class BetForm extends Component {
    render() {
        return (
            <div>
                <form>
                    $<input />
                    <submit>Bet</submit>
                </form>
            </div>
        );
    }
}

export class FightOverView extends Component {
    render() {
        if (!this.props.winner || !this.props.user) return null;
        return (
            <div>
                <div>{this.props.winner.name} won</div>
                <div>at {this.props.winner.odds} odds</div>
                <div>Your Bets</div>
                <table><tbody>
                    <tr>
                        <FightOverResult tubeman={this.props.tubeman1} bet={this.props.user.bet1} />
                        <FightOverResult tubeman={this.props.tubeman2} bet={this.props.user.bet2} />
                    </tr>
                </tbody></table>
            </div>
        );
    }
}

class FightOverResult extends Component {
    render() {
        if (!this.props.tubeman || !this.props.bet) return null;
        return (
            <td>
                <table><tbody>
                    <tr>
                        <td>{this.props.tubeman.name}:</td>
                        <td>${this.props.bet.amount}</td>
                    </tr>
                    <tr>
                        <td>Odds:</td>
                        <td>{this.props.tubeman.odds}</td>
                    </tr>
                    <tr>
                        <td>Pays:</td>
                        <td>${this.props.bet.payout}</td>
                    </tr>
                </tbody></table>
            </td>
        );
    }
}
