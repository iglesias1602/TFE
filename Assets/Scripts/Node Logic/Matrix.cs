public static class Matrix
{
    public static double[][] CreateMatrix(int size)
    {
        // Create a square matrix of the given size
        double[][] matrix = new double[size][];
        for (int i = 0; i < size; i++)
        {
            matrix[i] = new double[size];
        }
        return matrix;
    }

    public static double[][] MatrixInverse(double[][] matrix)
    {
        // Invert the given square matrix
        // Use a proper matrix inversion technique (e.g., Gaussian elimination)
        int n = matrix.Length;
        double[][] inverse = CreateMatrix(n);

        // Placeholder logic, replace with proper matrix inversion
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                inverse[i][j] = (i == j) ? 1.0 : 0.0;  // Identity matrix
            }
        }

        return inverse;
    }

    public static double[][] MatrixProduct(double[][] matrixA, double[][] matrixB)
    {
        // Multiply two matrices
        int rowsA = matrixA.Length;
        int colsA = matrixA[0].Length;
        int rowsB = matrixB.Length;
        int colsB = matrixB[0].Length;

        double[][] product = CreateMatrix(rowsA);  // Resulting product matrix

        for (int i = 0; i < rowsA; i++)
        {
            for (int j = 0; j < colsB; j++)
            {
                product[i][j] = 0;  // Initialize to zero
                for (int k = 0; k < colsA; k++)
                {
                    product[i][j] += matrixA[i][k] * matrixB[k][j];  // Matrix multiplication
                }
            }
        }

        return product;
    }
}
