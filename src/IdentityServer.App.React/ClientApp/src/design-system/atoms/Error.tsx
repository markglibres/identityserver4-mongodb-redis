import React from 'react';
import styled from 'styled-components';

interface ErrorProps {
    message: string | undefined;
}
const ErrorMessage = styled.label`
    color: red;
    font-size: 10px;
`;
export const Error: React.FC<ErrorProps> = ({ message }) => {
    return (
        <>
        {message && <ErrorMessage>{message}</ErrorMessage>}
        </>
    );
};
