<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="text" />
    <xsl:template match="/">
        <xsl:apply-templates />
    </xsl:template>
    <xsl:variable name="newline">
        <xsl:text>
        </xsl:text>
    </xsl:variable>
    <xsl:variable name="break">
        <xsl:text>
            
------------
            
        </xsl:text>
    </xsl:variable>
    <xsl:template match="test-run">
        <!-- Runtime Environment -->
        <xsl:value-of select="concat('Runtime Environment ', $newline)" />
        <xsl:value-of select="concat('   OS Version: ', test-suite/environment/@os-version[1], $newline)" />
        <xsl:value-of select="concat('  CLR Version: ', @clr-version, $break)" />
        <xsl:value-of select="concat('NUnit Version: ', @engine-version, $break)" />
        <!-- Test Run Summary -->
        <xsl:value-of select="concat('Test Run Summary ', $newline)" />
        <xsl:value-of select="concat(' Overall result: ', @result, $newline)" />
        <xsl:value-of select="concat(' Test Count: ', @total, ', Passed: ', @passed, ', Failed: ', @failed, ', Inconclusive: ', @inconclusive, ', Skipped: ', @skipped, $break)" />
        <!-- Times -->
        <xsl:value-of select="concat(' Start time: ', @start-time, $newline)" />
        <xsl:value-of select="concat('   End time: ', @end-time, $newline)" />
        <xsl:value-of select="concat('   Duration: ', format-number(@duration,'0.000'), ' seconds', $break)" />
        <xsl:apply-templates select="test-suite">
            <xsl:sort select="fullname" order="ascending" case-order="lower-first" />
        </xsl:apply-templates>
    </xsl:template>
    <xsl:template match="test-suite">
        <xsl:apply-templates select="test-suite">
            <xsl:sort select="fullname" order="ascending" case-order="lower-first" />
        </xsl:apply-templates>
        <xsl:value-of select="concat('# ', @fullname, ' : ', @result, ' (Duration: ', format-number(@duration,'0.000'), ' seconds)', $newline)" />
        <xsl:apply-templates select="test-case">
            <xsl:sort select="fullname" order="ascending" case-order="lower-first" />
        </xsl:apply-templates>
        <xsl:value-of select="concat('', $break)" />
    </xsl:template>
    <xsl:template match="test-case">
        <xsl:value-of select="concat('   ', @fullname, ' : ', @result, ' (Duration: ', format-number(@duration,'0.000'), ' seconds)', $newline)" />
    </xsl:template>
</xsl:stylesheet>