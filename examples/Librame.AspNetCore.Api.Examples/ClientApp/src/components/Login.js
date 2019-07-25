import React, { Component } from 'react';

export class Login extends Component {
    static displayName = Login.name;

    constructor(props) {
        super(props);
        this.state = { model: {}, loading: true };
        this.submitForm = this.submitForm.bind(this);
    }

    submitForm() {
        const query = `{
            mutation($user: LoginInput!) {
                login(user: $user) {
                    name
                    userId
                    token
                    message
                    isError
                }
            }
        }`;

        const variables = `{
            "user": {
                "name": "GraphUser",
                "password": "Password!123456",
                "rememberMe": false
            }
        }`;

        fetch('api/graphql', {
            method: 'post',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ query, variables })
        })
        .then(response => response.json())
        .then(data => {
            this.setState({ model: data, loading: false });
        });
    }

    static renderForecastsTable(forecasts) {

    }

    render() {
        let status = this.state.loading
            ? <p><em>Loading...</em></p>
            : FetchData.renderForecastsTable(this.state.forecasts);

        return (
            <div>
                <h1>����</h1>
                {status}
                <form id="login">
                    <div>
                        <label>�û���</label>
                        <input type='text' name='Name' />
                    </div>

                    <div>
                        <label>���룺</label>
                        <input type='password' name='Password' />
                    </div>

                    <div>
                        <label>��ס�ң�</label>
                        <input type='checkbox' name='RememberMe' />
                    </div>

                    <div>
                        <button className="btn btn-primary" onClick={this.submitForm}>����</button>
                    </div>
                </form>
            </div>
        );
    }
};