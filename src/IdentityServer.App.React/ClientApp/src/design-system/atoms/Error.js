import React from 'react';
import styled from 'styled-components';

const ErrorMessage = styled.label`
    color: red;
    font-size: 10px;
`;
export const Error = ({ message }) => {
    return (
        <>
        {message && <ErrorMessage>{message}</ErrorMessage>}
        </>
    );
};
